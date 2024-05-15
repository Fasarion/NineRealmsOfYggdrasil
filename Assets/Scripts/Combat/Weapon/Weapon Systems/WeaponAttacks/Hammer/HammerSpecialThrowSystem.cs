using System.Collections;
using System.Collections.Generic;
using Damage;
using Patrik;
using Player;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Weapon;


public partial struct HammerSpecialThrowSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerPositionSingleton>();
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<HammerSpecialConfig>();
        state.RequireForUpdate<RandomComponent>();
        
        state.RequireForUpdate<PlayerSpecialAttackInput>();
        state.RequireForUpdate<PlayerNormalAttackInput>();
    }


    public void OnUpdate(ref SystemState state)
    {
        HandleSpecialStart(ref state);
        HandleSpecialOnGoing(ref state);
        HandleSpecialStop(ref state);
    }
    
 
    void HandleSpecialStart(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingleton<WeaponAttackCaller>();
        var config = SystemAPI.GetSingletonRW<HammerSpecialConfig>();

        ChargeState currentChargeState = attackCaller.SpecialChargeInfo.chargeState;
        
        // start attack when stop charging
        if (currentChargeState != ChargeState.Stop) return;
        
        if (attackCaller.ActiveAttackData.WeaponType != WeaponType.Hammer) return;
        
        // commented out to avoid bug where throw is interupted by normal attack
        // //if (attackCaller.ActiveAttackData.AttackType != AttackType.Special) return;

        if (config.ValueRW.HasStarted) return;
        
        var hammerSpecialConfigEntity = SystemAPI.GetSingletonEntity<HammerSpecialConfig>();
        var cachedStageBuffer = SystemAPI.GetComponentRW<CachedChargeBuff>(hammerSpecialConfigEntity);

        config.ValueRW.HasStarted = true;
        config.ValueRW.HasReturned = false;
        config.ValueRW.TimeOfLastZap = 0;

        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerDirection = state.EntityManager.GetComponentData<LocalTransform>(playerEntity).Forward();
        playerDirection.y = 0;
        config.ValueRW.DirectionOfTravel = playerDirection;

        // Reset timer
        config.ValueRW.Timer = 0;
        
        foreach (var (animatorGO, knockBack, cachedDamage, hitSoundComponent) in SystemAPI
            .Query< GameObjectAnimatorPrefab, RefRW<KnockBackOnHitComponent>, CachedDamageComponent, RefRW<PlaySoundOnHittingComponent>>().WithAll<HammerComponent>())
        {
            // make hammer GO follow entity
            animatorGO.FollowEntity = true;

            config.ValueRW.KnockBackBeforeSpecial = knockBack.ValueRO.KnockDirection;
            knockBack.ValueRW.KnockDirection = KnockDirectionType.PerpendicularToPlayer;

            // cache damage
            cachedStageBuffer.ValueRW.Value.DamageModifier = cachedDamage.Value.DamageValue;
            
            // cache audio
            config.ValueRW.originalImpactAudio = hitSoundComponent.ValueRO.Value;
            hitSoundComponent.ValueRW.Value = config.ValueRO.throwImpactAudioData;
        }
        
        // Handle Throwing audio
        var audioBuffer = SystemAPI.GetSingletonBuffer<AudioBufferData>();
        audioBuffer.Add(new AudioBufferData { AudioData = config.ValueRO.throwingAudioData});

        //TODO: Don't call on attack stop until hammer is back and player has played its catch hammer animation
    }
    
   
    void HandleSpecialOnGoing(ref SystemState state)
    {
        var config = SystemAPI.GetSingletonRW<HammerSpecialConfig>();
        if (!config.ValueRO.HasStarted || config.ValueRO.HasReturned)
            return;
        
        var attackCaller = SystemAPI.GetSingleton<WeaponAttackCaller>();
        int chargeLevel = attackCaller.SpecialChargeInfo.Level;

        var hammerSpecialConfigEntity = SystemAPI.GetSingletonEntity<HammerSpecialConfig>();
        var chargeBuffer = SystemAPI.GetBuffer<ChargeBuffElement>(hammerSpecialConfigEntity);
        var cachedStageBuffer = SystemAPI.GetComponentRW<CachedChargeBuff>(hammerSpecialConfigEntity);

        float rangeModifier = chargeLevel < chargeBuffer.Length ? chargeBuffer[chargeLevel].Value.RangeModifier : 1f;
        float damageMod = chargeLevel < chargeBuffer.Length ? chargeBuffer[chargeLevel].Value.DamageModifier : 1f;

        config.ValueRW.DistanceToTravel = config.ValueRO.BaseDistanceToTravel * rangeModifier;
        
        var randomComponent = SystemAPI.GetSingletonRW<RandomComponent>();

        // Update timer
        float deltaTime = SystemAPI.Time.DeltaTime;
        config.ValueRW.Timer += deltaTime;
        
        // spawn zaps
        bool shouldSpawnZap = config.ValueRO.Timer > config.ValueRO.TimeOfLastZap + config.ValueRO.NextTimeBetweenZaps;
        if (shouldSpawnZap)
        {
            // instantiate
            var zap = state.EntityManager.Instantiate(config.ValueRO.ElectricChargePrefab);

            // fetch owner data
            Entity hammerEntity = SystemAPI.GetSingletonEntity<HammerComponent>();
            var weapon = state.EntityManager.GetComponentData<WeaponComponent>(hammerEntity);
            
            // set owner data
            state.EntityManager.SetComponentData(zap, new HasOwnerWeapon
            {
                OwnerEntity = hammerEntity,
                OwnerWasActive = weapon.InActiveState
            });
            
            // update when next zap should spawn
            config.ValueRW.TimeOfLastZap = config.ValueRO.Timer;
            config.ValueRW.NextTimeBetweenZaps = randomComponent.ValueRW.random
                .NextFloat(config.ValueRO.MinTimeBetweenZaps, config.ValueRW.MaxTimeBetweenZaps);
        }
        
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value;

        // Handle moving forwards
        if (!config.ValueRO.HasSwitchedBack)
        {
            var specialInput = SystemAPI.GetSingleton<PlayerSpecialAttackInput>();
            var normalInput = SystemAPI.GetSingleton<PlayerNormalAttackInput>();

            bool timerElapsed = config.ValueRO.Timer > config.ValueRO.TimeToSwitchBack;

            bool cancelInoutPressed = specialInput.KeyDown || normalInput.KeyDown;
            bool cancelInput = cancelInoutPressed && config.ValueRO.Timer > config.ValueRO.CancelDelayTime;
            
            // switch back when pressing input button to cancel or if timer has elapsed
            bool shouldSwitchBack = timerElapsed || cancelInput;
            if (shouldSwitchBack)
            {
                StartGoingBack(ref state, config);
            }
            // otherwise, hammer move forwards
            else
            {
                foreach (var (transform, animatorReference, animatorGO, hammer) in SystemAPI
                    .Query<RefRW<LocalTransform>, AnimatorReference, GameObjectAnimatorPrefab, HammerComponent>())
                {
                    transform.ValueRW.Position += config.ValueRO.DirectionOfTravel * config.ValueRO.TravelForwardSpeed * SystemAPI.Time.DeltaTime;
                
                    var directionToPlayer = playerPos - transform.ValueRO.Position;
                
                    var distance = math.length(directionToPlayer);
                    config.ValueRW.CurrentDistanceFromPlayer = distance;
                }
            }
        }
        else
        {
            // go back
            bool finishedReturning =
                (config.ValueRO.CurrentDistanceFromPlayer <= config.ValueRO.DistanceFromPlayerToGrab) ||
                config.ValueRO.Timer >= config.ValueRO.TimeToReturnAfterTurning;
            
            if (!finishedReturning)
            {
                foreach (var transform in SystemAPI
                    .Query<RefRW<LocalTransform>>().WithAll<HammerComponent>()) 
                {
                    var directionToPlayer = playerPos - transform.ValueRO.Position;
                    
                    var distance = math.length(directionToPlayer);
                    config.ValueRW.CurrentDistanceFromPlayer = distance;

                    float t = config.ValueRO.Timer * config.ValueRO.TimeToReturnAfterTurning;
                    transform.ValueRW.Position = math.lerp(transform.ValueRW.Position, playerPos, t);
                }
            }
            else
            {
                config.ValueRW.HasSwitchedBack = false;
                config.ValueRW.HasReturned = true;
                config.ValueRW.Timer = 0;
            }
        }
        
        LocalTransform hammerTrans = LocalTransform.Identity;
        
        foreach (var (transform, damageComponent) in SystemAPI
            .Query<RefRW<LocalTransform>, RefRW<CachedDamageComponent>>().WithAll<HammerComponent>())
        {
            // rotate hammer
            transform.ValueRW = transform.ValueRO.
                Rotate(quaternion.AxisAngle(config.ValueRO.RotationVector, deltaTime * config.ValueRO.RotationDegreesPerSecond));
            hammerTrans = transform.ValueRO;
            
            // set damage
            damageComponent.ValueRW.Value.DamageValue = damageMod * cachedStageBuffer.ValueRO.Value.DamageModifier;
        }
        
        // Make zaps follow hammer
        foreach (var transform in SystemAPI
            .Query<RefRW<LocalTransform>>().WithAll<HammerZapComponent>())
        {
            transform.ValueRW.Position = hammerTrans.Position;
        }
    }

    private static void StartGoingBack(ref SystemState state, RefRW<HammerSpecialConfig> config)
    {
        config.ValueRW.HasSwitchedBack = true;
        config.ValueRW.Timer = 0;
    }


    void HandleSpecialStop(ref SystemState state)
    {
        var config = SystemAPI.GetSingletonRW<HammerSpecialConfig>();
        if (!config.ValueRO.HasStarted || !config.ValueRO.HasReturned)
            return;
        
        var hammerSpecialConfigEntity = SystemAPI.GetSingletonEntity<HammerSpecialConfig>();
        var cachedStageBuffer = SystemAPI.GetComponentRW<CachedChargeBuff>(hammerSpecialConfigEntity);

        foreach (var (animatorGO, knockBack, cachedDamage, hitSoundComponent) in SystemAPI
            .Query< GameObjectAnimatorPrefab, RefRW<KnockBackOnHitComponent>, RefRW<CachedDamageComponent>, RefRW<PlaySoundOnHittingComponent>>().WithAll<HammerComponent>())
        {
            // make hammer entity follow GO again
            animatorGO.FollowEntity = false;
            knockBack.ValueRW.KnockDirection = config.ValueRO.KnockBackBeforeSpecial;
            
            // reset cached damage
            cachedDamage.ValueRW.Value.DamageValue = cachedStageBuffer.ValueRO.Value.DamageModifier;
            
            // reset hit sound
            hitSoundComponent.ValueRW.Value = config.ValueRO.originalImpactAudio;
        }

        // reset config
        config.ValueRW.HasReturned = true;
        config.ValueRW.HasStarted = false;
        config.ValueRW.HasSwitchedBack = false;

        var weaponCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        weaponCaller.ValueRW.ReturnWeapon = true;
    }
}
using System.Collections;
using System.Collections.Generic;
using Damage;
using Destruction;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Content;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(AddKnockBackOnHitSystem))]
[UpdateBefore(typeof(HitStopSystem))]
[BurstCompile]
public partial struct IceRingSystem : ISystem
{
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SwordComponent>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<IceRingAbility>();
        state.RequireForUpdate<IceRingConfig>();
        state.RequireForUpdate<PlayerPositionSingleton>();
        state.RequireForUpdate<PlayerSpecialAttackInput>();
        state.RequireForUpdate<AudioBufferData>();
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingletonRW<IceRingConfig>();
        var input = SystemAPI.GetSingleton<PlayerSpecialAttackInput>();
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
       // var stageBuffer = SystemAPI.GetSingletonBuffer<IceRingStageElement>(false);
        var configEntity = SystemAPI.GetSingletonEntity<IceRingConfig>();
        var attackCaller = SystemAPI.GetSingleton<WeaponAttackCaller>();

        var iceRingEntity = SystemAPI.GetSingletonEntity<IceRingConfig>();
        var stageBuffer = SystemAPI.GetBuffer<ChargeBuffElement>(iceRingEntity);
        var cachedStageBuffer = SystemAPI.GetComponentRW<CachedChargeBuff>(iceRingEntity);

        var audioBuffer = SystemAPI.GetSingletonBuffer<AudioBufferData>();

        int chargeLevel = attackCaller.SpecialChargeInfo.Level;
        
        foreach (var (ability, transform, chargeTimer, entity) in
                 SystemAPI.Query<RefRW<IceRingAbility>, RefRW<LocalTransform>, RefRW<ChargeTimer>>()
                     .WithEntityAccess())
        {
            var damageComponent = state.EntityManager.GetComponentData<CachedDamageComponent>(configEntity);

            //set up ability
            if (!ability.ValueRW.isInitialized)
            {
                chargeTimer.ValueRW.maxChargeTime = config.ValueRO.maxChargeTime;
                ability.ValueRW.isInitialized = true;
                transform.ValueRW.Rotation = Quaternion.Euler(0, config.ValueRO.chargeAreaVfxHeightOffset, 0f);
              //  config.ValueRW.ogCachedDamageValue = damageComponent.Value.DamageValue;

                cachedStageBuffer.ValueRW.Value.DamageModifier = damageComponent.Value.DamageValue;
                
                
                // fetch owner data
                Entity swordEntity = SystemAPI.GetSingletonEntity<SwordComponent>();
                var weapon = state.EntityManager.GetComponentData<WeaponComponent>(swordEntity);
            
                // set owner data
                state.EntityManager.SetComponentData(iceRingEntity, new HasOwnerWeapon
                {
                    OwnerEntity = swordEntity,
                    OwnerWasActive = weapon.InActiveState
                });
                
                // play charging audio
                var audioElement = new AudioBufferData() {AudioData = config.ValueRO.chargeAudioData};
                audioBuffer.Add(audioElement);
            }

            float damageMod = 1;
            float rangeMod = 1;
            
            // set correct charge values if in range
            if (chargeLevel < stageBuffer.Length)
            {
                damageMod = stageBuffer[chargeLevel].Value.DamageModifier;
                rangeMod = stageBuffer[chargeLevel].Value.RangeModifier;
            }

            damageComponent.Value.DamageValue = damageMod * cachedStageBuffer.ValueRO.Value.DamageModifier;
           // damageComponent.Value.DamageValue = damageMod * config.ValueRO.ogCachedDamageValue;
            ecb.SetComponent(configEntity, damageComponent);
            
            var area = rangeMod;
            

            transform.ValueRW.Position = playerPos.Value + new float3(0, -.5f, 0);
            transform.ValueRW.Scale = area * .5f;

            // spawn ability
            if (attackCaller.SpecialChargeInfo.chargeState == ChargeState.Stop)
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);
                
                var effect = state.EntityManager.Instantiate(config.ValueRO.abilityPrefab);
                
                state.EntityManager.SetComponentData(effect, new IceRingAbility
                {
                    area = area
                });
            }
        }
        

        foreach (var (ability, timer, entity) in
                 SystemAPI.Query<RefRW<IceRingAbility>, RefRW<TimerObject>>()
                     .WithEntityAccess())
        {
            if (ability.ValueRO.CurrentRingCount >= config.ValueRO.MaxRings)
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);
                continue;
            }

            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;

            if (timer.ValueRO.currentTime >= config.ValueRO.MultiSpawnDelayTime * ability.ValueRO.CurrentRingCount)
            {
                var effect = state.EntityManager.Instantiate(config.ValueRO.projectilePrefab);
                var area = ability.ValueRO.area +
                           config.ValueRO.MultiSpawnSizeGrowth * ability.ValueRO.CurrentRingCount;

                state.EntityManager.SetComponentData(effect, new LocalTransform
                {
                    Position = playerPos.Value + new float3(0, config.ValueRO.abilityVfxHeightOffset, 0),
                    Rotation = Quaternion.Euler(-90f, 0f, 0f),
                    Scale = area * .5f
                });
                state.EntityManager.SetComponentData(effect, new IceRingProjectile
                {
                    Area = area,
                });
                state.EntityManager.SetComponentData(effect, new UpdateStatsComponent
                {
                    EntityToTransferStatsFrom = iceRingEntity,
                });

                // play impact audio
                var audioElement = new AudioBufferData() {AudioData = config.ValueRO.impactAudioData};
                audioBuffer.Add(audioElement);
            
                ability.ValueRW.CurrentRingCount++;
            }
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();

    }
}

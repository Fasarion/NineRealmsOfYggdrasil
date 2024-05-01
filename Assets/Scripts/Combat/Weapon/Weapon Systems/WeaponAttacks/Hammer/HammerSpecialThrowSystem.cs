using System.Collections;
using System.Collections.Generic;
using Damage;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;


public partial struct HammerSpecialThrowSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<HammerSpecialConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        HandleSpecialStart(ref state);
        HandleSpecialOnGoing(ref state);
        HandleSpecialStop(ref state);
    }
    
    [BurstCompile]
    void HandleSpecialStart(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingleton<WeaponAttackCaller>();
        var config = SystemAPI.GetSingletonRW<HammerSpecialConfig>();
        
        bool shouldAttack = attackCaller.ShouldStartActiveAttack(WeaponType.Hammer, AttackType.Special);
        if (!shouldAttack) return;

        config.ValueRW.HasStarted = true;
        config.ValueRW.HasReturned = false;

        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerDirection = state.EntityManager.GetComponentData<LocalTransform>(playerEntity).Forward();
        playerDirection.y = 0;
        config.ValueRW.DirectionOfTravel = playerDirection;

        // Reset timer
        config.ValueRW.Timer = 0;
        
        // make hammer GO follow entity
        foreach (var (animatorGO, knockBack) in SystemAPI
            .Query< GameObjectAnimatorPrefab, RefRW<KnockBackOnHitComponent>>().WithAll<HammerComponent>())
        {
            animatorGO.FollowEntity = true;

            config.ValueRW.KnockBackBeforeSpecial = knockBack.ValueRO.KnockDirection;
            knockBack.ValueRW.KnockDirection = KnockDirectionType.PerpendicularToPlayer;
        }
        
        //TODO: Don't call on attack stop until hammer is back and player has played its catch hammer animation
    }
    
    [BurstCompile]
    void HandleSpecialOnGoing(ref SystemState state)
    {
        var config = SystemAPI.GetSingletonRW<HammerSpecialConfig>();
        if (!config.ValueRO.HasStarted || config.ValueRO.HasReturned)
            return;
        
        // Update timer
        float deltaTime = SystemAPI.Time.DeltaTime;
        config.ValueRW.Timer += deltaTime;
        
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value;

        // Handle moving forwards
        if (!config.ValueRO.HasSwitchedBack)
        {
            // go forwards
            if (config.ValueRO.Timer < config.ValueRO.TimeToSwitchBack)
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
            else
            {
                config.ValueRW.HasSwitchedBack = true;
                config.ValueRW.Timer = 0;
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
        
        // Rotate Hammer
        foreach (var transform in SystemAPI
            .Query<RefRW<LocalTransform>>().WithAll<HammerComponent>())
        {
            transform.ValueRW = transform.ValueRO.RotateY(deltaTime * config.ValueRO.ResolutionsPerSecond);
        }
    }
    
    [BurstCompile]
    void HandleSpecialStop(ref SystemState state)
    {
        var config = SystemAPI.GetSingletonRW<HammerSpecialConfig>();
        if (!config.ValueRO.HasStarted || !config.ValueRO.HasReturned)
            return;

        // make hammer entity follow GO again
        foreach (var (animatorGO, knockBack) in SystemAPI
            .Query< GameObjectAnimatorPrefab, RefRW<KnockBackOnHitComponent>>().WithAll<HammerComponent>())
        {
            animatorGO.FollowEntity = false;
            
            knockBack.ValueRW.KnockDirection = config.ValueRO.KnockBackBeforeSpecial;
        }

        // reset config
        config.ValueRW.HasReturned = true;
        config.ValueRW.HasStarted = false;
        config.ValueRW.HasSwitchedBack = false;

        var weaponCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        weaponCaller.ValueRW.ResetWeaponCurrentWeaponTransform = true;
    }
}
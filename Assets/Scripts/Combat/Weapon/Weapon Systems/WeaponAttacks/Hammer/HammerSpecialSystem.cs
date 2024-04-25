using System.Collections;
using System.Collections.Generic;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
public partial struct HammerSpecialSystem : ISystem
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
        config.ValueRW.DirectionOfTravel = playerDirection;

        // Reset timer
        config.ValueRW.Timer = 0;
        
        // make hammer GO follow entity
        foreach (var (transform, animatorReference, animatorGO, hammer) in SystemAPI
            .Query<LocalTransform, AnimatorReference, GameObjectAnimatorPrefab, HammerComponent>())
        {
            animatorGO.FollowEntity = true;
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
        config.ValueRW.Timer += SystemAPI.Time.DeltaTime;

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
            if (config.ValueRO.Timer < config.ValueRO.TimeToReturnAfterTurning)
            {

                var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value;

                foreach (var transform in SystemAPI
                    .Query<RefRW<LocalTransform>>().WithAll<HammerComponent>()) 
                {
                    // var directionToPlayer = playerPos - transform.ValueRO.Position;
                    //
                    // var distance = math.length(directionToPlayer);
                    // var directionNorm = math.normalize(directionToPlayer);
                    //
                    // var travelBackSpeed = distance / config.ValueRO.TimeToReturnAfterTurning;
                    
                  //  transform.ValueRW.Position += directionNorm * travelBackSpeed * SystemAPI.Time.DeltaTime;

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
    }
    
    [BurstCompile]
    void HandleSpecialStop(ref SystemState state)
    {
        var config = SystemAPI.GetSingletonRW<HammerSpecialConfig>();
        if (!config.ValueRO.HasStarted || !config.ValueRO.HasReturned)
            return;
        
        
        
        // make hammer entity follow GO again
        foreach (var (transform, animatorReference, animatorGO, hammer) in SystemAPI
            .Query<LocalTransform, AnimatorReference, GameObjectAnimatorPrefab, HammerComponent>())
        {
            animatorGO.FollowEntity = false;
        }

        // reset config
        config.ValueRW.HasReturned = true;
        config.ValueRW.HasStarted = false;
        config.ValueRW.HasSwitchedBack = false;

        var weaponCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        weaponCaller.ValueRW.ResetWeaponCurrentWeaponTransform = true;
    }
    
    
}

// public partial class ResetHammerSystem : SystemBase
// {
//     protected override void OnUpdate()
//     {
//         
//         
//         
//         var config = SystemAPI.GetSingletonRW<HammerSpecialConfig>();
//         if (!config.ValueRO.HasStarted || !config.ValueRO.HasReturned)
//             return;
//         
//         // make hammer entity follow GO again
//         foreach (var (transform, animatorReference, animatorGO, hammer) in SystemAPI
//             .Query<LocalTransform, AnimatorReference, GameObjectAnimatorPrefab, HammerComponent>())
//         {
//             animatorGO.FollowEntity = false;
//         }
//
//         // reset config
//         config.ValueRW.HasReturned = false;
//         config.ValueRW.HasStarted = false;
//         config.ValueRW.HasSwitchedBack = false;
//
//         var weaponCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
//         weaponCaller.ValueRW.ResetWeaponCurrentWeaponTransform = true;
//
//     }
// }

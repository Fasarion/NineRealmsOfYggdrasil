using System.Collections;
using System.Collections.Generic;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
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
        HandleSpecialStop(ref state);
        HandleSpecialOnGoing(ref state);
    }
    
    [BurstCompile]
    void HandleSpecialStart(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingleton<WeaponAttackCaller>();
        var config = SystemAPI.GetSingletonRW<HammerSpecialConfig>();

        bool shouldAttack = attackCaller.ShouldStartActiveAttack(WeaponType.Hammer, AttackType.Special);
        if (!shouldAttack) return;

        config.ValueRW.HasStarted = true;

        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerDirection = state.EntityManager.GetComponentData<LocalTransform>(playerEntity).Forward();
        config.ValueRW.DirectionOfTravel = playerDirection;
        
        
        foreach (var (transform, animatorReference, animatorGO, hammer) in SystemAPI
            .Query<LocalTransform, AnimatorReference, GameObjectAnimatorPrefab, HammerComponent>())
        {
            animatorGO.FollowEntity = true;
        }
    }
    
    [BurstCompile]
    void HandleSpecialOnGoing(ref SystemState state)
    {
        var config = SystemAPI.GetSingletonRW<HammerSpecialConfig>();
        if (!config.ValueRO.HasStarted)
            return;
        
        foreach (var (transform, animatorReference, animatorGO, hammer) in SystemAPI
            .Query<RefRW<LocalTransform>, AnimatorReference, GameObjectAnimatorPrefab, HammerComponent>())
        {
            transform.ValueRW.Position += config.ValueRO.DirectionOfTravel * config.ValueRO.TravelForwardSpeed * SystemAPI.Time.DeltaTime;
        }
    }
    
    [BurstCompile]
    void HandleSpecialStop(ref SystemState state)
    {
        
    }
}

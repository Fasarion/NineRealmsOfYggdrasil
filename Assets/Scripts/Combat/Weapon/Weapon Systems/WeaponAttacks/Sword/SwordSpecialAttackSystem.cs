using System.Collections;
using System.Collections.Generic;
using Damage;
using Patrik;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public partial struct SwordSpecialAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SwordStatsTag>();
        state.RequireForUpdate<BasePlayerStatsTag>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<SwordComponent>();
        state.RequireForUpdate<AudioBufferData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

        if (!attackCaller.ValueRO.ShouldAttackWithType(WeaponType.Sword, AttackType.Special))
            return;

        attackCaller.ValueRW.shouldActiveAttack = false;

        Debug.Log("Sword Special");
        return;

        //CollisionCheck
        var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        var hits = new NativeList<DistanceHit>(state.WorldUpdateAllocator);

        var hammerStatsEntity = SystemAPI.GetSingletonEntity<HammerStatsTag>();
        var hammerStatsComponent = state.EntityManager.GetComponentData<WeaponStatsComponent>(hammerStatsEntity);

        var playerStatsEntity = SystemAPI.GetSingletonEntity<BasePlayerStatsTag>();
        var playerStatsComponent = state.EntityManager.GetComponentData<WeaponStatsComponent>(playerStatsEntity);

        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();

        float totalArea = playerStatsComponent.baseArea + hammerStatsComponent.baseArea;

        foreach (var (weapon, buffer, entity) in
            SystemAPI.Query<WeaponComponent, DynamicBuffer<HitBufferElement>>()
                .WithAll<ActiveWeapon>()
                .WithEntityAccess())
        {
            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Patrik;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

[BurstCompile]
public partial struct HammerUltimateAttackSystem : ISystem
{
    private CollisionFilter _detectionFilter;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HammerStatsTag>();
        state.RequireForUpdate<BasePlayerStatsTag>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<HammerComponent>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        _detectionFilter = new CollisionFilter
        {
            BelongsTo = 1, // Projectile
            CollidesWith = 1 << 1 // Enemy
        };
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

        if (!attackCaller.ValueRO.ShouldAttackWithType(WeaponType.Hammer, AttackType.Ultimate))
            return;

        attackCaller.ValueRW.shouldActiveAttack = false;
        Debug.Log("ult!");
    }
}
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
using UnityEditor.Timeline.Actions;
using UnityEngine;

[BurstCompile]
public partial struct HammerNormalAttackSystem : ISystem
{
    private CollisionFilter _detectionFilter;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HammerStatsTag>();
        state.RequireForUpdate<BasePlayerStatsTag>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<HammerComponent>();
        state.RequireForUpdate<RandomComponent>();

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

        if (!attackCaller.ValueRO.ShouldAttackWithType(WeaponType.Hammer, AttackType.Normal))
            return;

        attackCaller.ValueRW.shouldActiveAttack = false;

        var hammerStatsEntity = SystemAPI.GetSingletonEntity<HammerStatsTag>();
        var hammerStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(hammerStatsEntity);
        
        var playerStatsEntity = SystemAPI.GetSingletonEntity<BasePlayerStatsTag>();
        var playerStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(playerStatsEntity);

        var randomFloat = SystemAPI.GetSingletonRW<RandomComponent>().ValueRW.random.NextFloat();

        int combo = attackCaller.ValueRO.currentCombo;
        float totalDamage = CombatStats.GetTotalDamageWithCrit(playerStatsComponent, hammerStatsComponent, AttackType.Normal, randomFloat, combo);
        float totalKnockBack = CombatStats.GetTotalKnockBack(playerStatsComponent, hammerStatsComponent, AttackType.Normal, combo);

        foreach (var (damageComp, knockBackComp) in SystemAPI
            .Query<RefRW<DamageOnTriggerComponent>, RefRW<KnockBackForce>>()
            .WithAll<HammerComponent>())
        {
            damageComp.ValueRW.DamageValue = totalDamage;
            knockBackComp.ValueRW.Value = totalKnockBack; 
        }
    }
}

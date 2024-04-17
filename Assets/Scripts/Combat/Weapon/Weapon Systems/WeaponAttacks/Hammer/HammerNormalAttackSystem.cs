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
//
// [BurstCompile]
// public partial struct HammerNormalAttackSystem : ISystem
// {
//     [BurstCompile]
//     public void OnCreate(ref SystemState state)
//     {
//         state.RequireForUpdate<HammerStatsTag>();
//         state.RequireForUpdate<BasePlayerStatsTag>();
//         state.RequireForUpdate<WeaponAttackCaller>();
//         state.RequireForUpdate<HammerComponent>();
//     }
//
//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
//
//         if (!attackCaller.ValueRO.ShouldAttackWithType(WeaponType.Hammer, AttackType.Normal))
//             return;
//
//         attackCaller.ValueRW.shouldActiveAttack = false;
//
//         var hammerStatsEntity = SystemAPI.GetSingletonEntity<HammerStatsTag>();
//         var hammerStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(hammerStatsEntity);
//         
//         var playerStatsEntity = SystemAPI.GetSingletonEntity<BasePlayerStatsTag>();
//         var playerStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(playerStatsEntity);
//         
//         int combo = attackCaller.ValueRO.currentCombo;
//         float totalDamage = CombatStats.GetDamageWithoutCrit(playerStatsComponent, hammerStatsComponent, AttackType.Normal, combo);
//         float totalCritRate = CombatStats.GetCriticalRate(playerStatsComponent, hammerStatsComponent, AttackType.Normal);
//         float totalCritMod = CombatStats.GetCriticalModifier(playerStatsComponent, hammerStatsComponent, AttackType.Normal);
//         float totalKnockBack = CombatStats.GetKnockBack(playerStatsComponent, hammerStatsComponent, AttackType.Normal, combo);
//
//         DamageContents damageContents = new DamageContents()
//         {
//             DamageValue = totalDamage,
//             CriticalRate = totalCritRate,
//             CriticalModifier = totalCritMod,
//         };
//         
//         foreach (var (damageComp, knockBackComp) in SystemAPI
//             .Query<RefRW<DamageOnTriggerComponent>, RefRW<KnockBackForce>>()
//             .WithAll<HammerComponent>())
//         {
//             damageComp.ValueRW.Value = damageContents;
//             knockBackComp.ValueRW.Value = totalKnockBack; 
//         }
//     }
// }

public partial struct WeaponStatsTransferSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HammerStatsTag>();
        state.RequireForUpdate<BasePlayerStatsTag>();
        state.RequireForUpdate<WeaponAttackCaller>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

        if (!attackCaller.ValueRO.shouldActiveAttack)
            return;
        
        // Debug.Log("Should active attack");
        
        // var hammerStatsEntity = SystemAPI.GetSingletonEntity<HammerStatsTag>();
        // var hammerStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(hammerStatsEntity);
        //
        // var playerStatsEntity = SystemAPI.GetSingletonEntity<BasePlayerStatsTag>();
        // var playerStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(playerStatsEntity);
        //
        // int combo = attackCaller.ValueRO.currentCombo;
        // float totalDamage = CombatStats.GetDamageWithoutCrit(playerStatsComponent, hammerStatsComponent, AttackType.Normal, combo);
        // float totalCritRate = CombatStats.GetCriticalRate(playerStatsComponent, hammerStatsComponent, AttackType.Normal);
        // float totalCritMod = CombatStats.GetCriticalModifier(playerStatsComponent, hammerStatsComponent, AttackType.Normal);
        // float totalKnockBack = CombatStats.GetKnockBack(playerStatsComponent, hammerStatsComponent, AttackType.Normal, combo);
        //
        // DamageContents damageContents = new DamageContents()
        // {
        //     DamageValue = totalDamage,
        //     CriticalRate = totalCritRate,
        //     CriticalModifier = totalCritMod,
        // };
        //
        // foreach (var (damageComp, knockBackComp) in SystemAPI
        //     .Query<RefRW<DamageOnTriggerComponent>, RefRW<KnockBackForce>>()
        //     .WithAll<HammerComponent>())
        // {
        //     damageComp.ValueRW.Value = damageContents;
        //     knockBackComp.ValueRW.Value = totalKnockBack; 
        // }
    }
}

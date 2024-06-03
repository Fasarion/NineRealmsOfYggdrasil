using System.Collections;
using System.Collections.Generic;
using Patrik;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(SwordUltimateAttackSystem))]
[UpdateAfter(typeof(SwordSpecialAttackSystem))]
[BurstCompile]
public partial struct SwordNormalAttackSystem : ISystem
{
    private bool hasRecorded;
    private int attackType;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SwordComboAbilityConfig>();
        state.RequireForUpdate<SwordComponent>();
        state.RequireForUpdate<WeaponAttackCaller>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        var config = SystemAPI.GetSingleton<SwordComboAbilityConfig>();
        
        if (attackCaller.ValueRO.ShouldStartActiveAttack(WeaponType.Sword, AttackType.Normal))
        {
            if (attackType == 1)
            {
                state.EntityManager.Instantiate(config.SwordComboAbilityPrefab);
            }
            
            if (attackType == 0) attackType = 1;
            else attackType = 0;
            
            attackCaller.ValueRW.ActiveAttackData.ShouldStart = false;
            if (!hasRecorded)
            {
                hasRecorded = true;
                var swordEntity = SystemAPI.GetSingletonEntity<SwordComponent>();
                state.EntityManager.AddComponent<ShouldRecordSwordTrajectoryComponent>(swordEntity);
            }


        }
    }
}

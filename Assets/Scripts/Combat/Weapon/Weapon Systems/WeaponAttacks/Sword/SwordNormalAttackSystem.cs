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
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WeaponAttackCaller>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        
        if (attackCaller.ValueRO.ShouldActiveAttackWithType(WeaponType.Sword, AttackType.Normal))
        {
            attackCaller.ValueRW.shouldActiveAttack = false;
        }
    }
}

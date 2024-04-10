using System.Collections;
using System.Collections.Generic;
using Patrik;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct HammerNormalAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<HammerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

        if (!attackCaller.ValueRO.ShouldActiveAttackWithType(WeaponType.Hammer)) 
            return;
        
        Debug.Log("is running");
        attackCaller.ValueRW.shouldActiveAttack = false;
    }
}

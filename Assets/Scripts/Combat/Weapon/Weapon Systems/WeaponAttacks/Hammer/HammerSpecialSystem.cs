using System.Collections;
using System.Collections.Generic;
using Patrik;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public partial struct HammerSpecialSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WeaponAttackCaller>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingleton<WeaponAttackCaller>();

        bool shouldAttack = attackCaller.ShouldStartActiveAttack(WeaponType.Hammer, AttackType.Special);
        if (!shouldAttack) return;
        
        
        
    }
}

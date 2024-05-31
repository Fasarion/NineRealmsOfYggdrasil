using System.Collections;
using System.Collections.Generic;
using Patrik;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct HammerSpecialProjectileAbilitySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HammerSpecialConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        var config = SystemAPI.GetSingleton<HammerSpecialConfig>();
        bool isCharging = attackCaller.ValueRO.SpecialChargeInfo.IsChargingWithWeapon(WeaponType.Hammer);
    }
}

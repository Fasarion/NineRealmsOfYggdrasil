using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public partial struct HammerSpecialIndicationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<HammerComponent>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

        if (!attackCaller.ValueRO.SpecialChargeInfo.IsChargingWithWeapon(WeaponType.Hammer))
            return;
        
        // TODO: spawn indicator, 
        
    }
}
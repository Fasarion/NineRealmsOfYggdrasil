
using Movement;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Weapon;

[DisableAutoCreation]
public partial struct BirdSpecialAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WeaponAttackCaller>();
        
        state.RequireForUpdate<PlayerRotationSingleton>();
        state.RequireForUpdate<PlayerPositionSingleton>();
        
        state.RequireForUpdate<PlayerTag>();
        
        // spcial atk config
        state.RequireForUpdate<BirdNormalAttackConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingleton<WeaponAttackCaller>();
        var config = SystemAPI.GetSingletonRW<HammerSpecialConfig>();

        ChargeState currentChargeState = attackCaller.SpecialChargeInfo.chargeState;
    }
}

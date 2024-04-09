using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
public partial struct ResetInputSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerMoveInput>();
        
        state.RequireForUpdate<PlayerNormalAttackInput>();
        state.RequireForUpdate<PlayerSpecialAttackInput>();
        state.RequireForUpdate<PlayerUltimateAttackInput>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        // Reset move input
        var moveInput = SystemAPI.GetSingletonRW<PlayerMoveInput>();
        moveInput.ValueRW.Value = float2.zero;

        // Reset fire input
        var normalAttackInput = SystemAPI.GetSingletonRW<PlayerNormalAttackInput>();
        normalAttackInput.ValueRW.KeyPressed = false;
        var specialAttackInput = SystemAPI.GetSingletonRW<PlayerSpecialAttackInput>();
        specialAttackInput.ValueRW.KeyPressed = false;
        var ultimateAttackInput = SystemAPI.GetSingletonRW<PlayerUltimateAttackInput>();
        ultimateAttackInput.ValueRW.KeyPressed = false;
        
        // Reset weapon input
        var weapon1 = SystemAPI.GetSingletonRW<WeaponOneInput>();
        weapon1.ValueRW.KeyPressed = false;
        var weapon2 = SystemAPI.GetSingletonRW<WeaponTwoInput>();
        weapon2.ValueRW.KeyPressed = false;
        var weapon3 = SystemAPI.GetSingletonRW<WeaponThreeInput>();
        weapon3.ValueRW.KeyPressed = false;

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

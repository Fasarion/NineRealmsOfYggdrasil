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
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        // Reset move input
        var moveInput = SystemAPI.GetSingletonRW<PlayerMoveInput>();
        moveInput.ValueRW.Value = float2.zero;

        // Reset fire input
        var fireInput = SystemAPI.GetSingletonRW<PlayerFireInput>();
        fireInput.ValueRW.FireKeyPressed = false;
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

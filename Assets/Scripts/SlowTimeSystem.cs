using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public partial struct SlowTimeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ShouldSlowTimeComponent>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingletonRW<SlowTimeSingleton>();

        if (config.ValueRO.IsTimeSlowed) return;
        
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        foreach (var (timeSlow, entity) in
                 SystemAPI.Query<ShouldSlowTimeComponent>()
                     .WithEntityAccess())
        {
            ecb.RemoveComponent<ShouldSlowTimeComponent>(entity);
            config.ValueRW.IsTimeSlowed = true;
            config.ValueRW.CurrentSlowFactor = 1;
            config.ValueRW.SlowFactorTarget = timeSlow.SlowFactor;
            config.ValueRW.FadeInSpeed = timeSlow.FadeInTime;
            config.ValueRW.FadeOutSpeed = timeSlow.FadeOutTime;
            config.ValueRW.SlowTargetDuration = timeSlow.SlowDuration;
            config.ValueRW.ShouldSlowTime = true;
            break;
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

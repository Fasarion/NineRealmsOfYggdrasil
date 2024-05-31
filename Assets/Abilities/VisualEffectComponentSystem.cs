using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct VisualEffectComponentSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        foreach (var (effectComponent, transform, entity) in
                 SystemAPI.Query<RefRW<VisualEffectComponent>, RefRW<LocalTransform>>()
                     .WithEntityAccess())
        {
            
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

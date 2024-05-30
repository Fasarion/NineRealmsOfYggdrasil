using System.Collections;
using System.Collections.Generic;
using Destruction;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
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
            if (effectComponent.ValueRO.TimerTime > effectComponent.ValueRO.ActiveTime)
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);
                continue;
            }
            
            if (state.EntityManager.HasComponent<LocalTransform>(effectComponent.ValueRO.EntityToFollow) && effectComponent.ValueRO.ShouldFollowParentEntity)
            {
                var entityToFollowTransform = state.EntityManager.GetComponentData<LocalTransform>(effectComponent.ValueRO.EntityToFollow);
                var entityToFollowPos = entityToFollowTransform.Position;
                
                var offset = effectComponent.ValueRO.SpawnOffset;
                transform.ValueRW.Position = entityToFollowPos + offset;
                
                transform.ValueRW.Rotation = entityToFollowTransform.Rotation;
            }

            effectComponent.ValueRW.TimerTime += SystemAPI.Time.DeltaTime;
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

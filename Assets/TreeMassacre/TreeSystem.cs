using System.Collections;
using System.Collections.Generic;
using Damage;
using Destruction;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(CombatSystemGroup))]
[UpdateBefore(typeof(AddDamageBufferElementOnTriggerSystem))]
[BurstCompile]
public partial struct TreeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
        foreach (var (ability, entity) in
                 SystemAPI.Query<TreeComponent>()
                     .WithEntityAccess())
        {
            var buffer = state.EntityManager.GetBuffer<HitBufferElement>(entity);
            if (buffer.Length == 0) return;
            
            buffer.Clear();
            
            ecb.AddComponent<ShouldBeDestroyed>(entity);
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

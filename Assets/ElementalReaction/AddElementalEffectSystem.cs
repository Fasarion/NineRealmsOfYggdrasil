using System.Collections;
using System.Collections.Generic;
using Damage;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

[UpdateInGroup(typeof(CombatSystemGroup))]
public partial struct AddElementalEffectSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }
        
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
        // var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        //
        // foreach (var ( hitBuffer, iceApplierComponent) 
        //          in SystemAPI.Query<DynamicBuffer<HitBufferElement>, ElementalShouldApplyIceComponent>())
        // {
        //     foreach (var hit in hitBuffer)
        //     {
        //         if (hit.IsHandled) continue;
        //         if (!state.EntityManager.HasComponent<ElementalIceEffectComponent>(hit.HitEntity))
        //         {
        //             state.EntityManager.AddComponent<ElementalIceEffectComponent>(hit.HitEntity);
        //         }
        //
        //
        //     }
        // }
    }
}

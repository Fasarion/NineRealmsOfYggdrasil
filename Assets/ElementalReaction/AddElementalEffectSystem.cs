using System.Collections;
using System.Collections.Generic;
using Damage;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

[UpdateBefore(typeof(ApplyDamageSystem))]
[UpdateInGroup(typeof(CombatSystemGroup))]
public partial struct AddElementalEffectSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ElementalReactionsConfig>();
    }
        
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var config = SystemAPI.GetSingleton<ElementalReactionsConfig>();
        
        foreach (var ( hitBuffer, _) 
                 in SystemAPI.Query<DynamicBuffer<HitBufferElement>, ElementalShouldApplyIceComponent>())
        {
            foreach (var hit in hitBuffer)
            {
                if (hit.IsHandled) continue;
                if (!state.EntityManager.HasComponent<ElementalIceEffectComponent>(hit.HitEntity))
                {
                    ecb.AddComponent<ElementalIceEffectComponent>(hit.HitEntity);
                    //ecb.SetComponent(hit.HitEntity, new ElementalIceEffectComponent());
                }
                else
                {
                    int stacks = 0;
                    if (!state.EntityManager.HasComponent<ElementalFreezeEffectComponent>(hit.HitEntity))
                    {
                        ecb.AddComponent<ElementalFreezeEffectComponent>(hit.HitEntity);
                        stacks = 1;
                    }
                    else
                    {
                        stacks = state.EntityManager.GetComponentData<ElementalFreezeEffectComponent>(hit.HitEntity).Stacks +
                                 1;
                        if (stacks > config.MaxFreezeStacks) stacks = (int)config.MaxFreezeStacks;
                    }
                    
                    ecb.SetComponent(hit.HitEntity, new ElementalFreezeEffectComponent
                    {
                        Stacks = stacks,
                        HasBeenApplied = false,
                        CurrentDurationTime = 0,
                    });
                }
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();

    }
}

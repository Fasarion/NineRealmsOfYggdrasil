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
        int stacks = 0;
        
        
        ApplyFireEffect(state, ecb, config);
        ApplyLightningEffect(state, ecb, config);
        ApplyIceEffect(state, ecb, config);
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    private void ApplyIceEffect(SystemState state, EntityCommandBuffer ecb, ElementalReactionsConfig config)
    {
        //Apply Ice
        foreach (var ( hitBuffer, _) 
                 in SystemAPI.Query<DynamicBuffer<HitBufferElement>, ElementalShouldApplyIceComponent>())
        {
            foreach (var hit in hitBuffer)
            {
                if (hit.IsHandled) continue;
                if (!state.EntityManager.HasComponent<ElementalIceEffectComponent>(hit.HitEntity))
                {
                    //Add Ice effect
                    ecb.AddComponent<ElementalIceEffectComponent>(hit.HitEntity);
                }
                else
                {
                    //Add Freeze Effect
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
                    
                    //Add Conduct Effect
                    if (state.EntityManager.HasComponent<ElementalLightningEffectComponent>(hit.HitEntity))
                    {
                        ApplyConductEffect(state, hit, ecb, config);
                    }
                    
                    //Add Vulnerable Effect
                    if (state.EntityManager.HasComponent<ElementalFireEffectComponent>(hit.HitEntity))
                    {
                        ApplyVulnerableEffect(state, hit, ecb, config);
                    }
                }
            }
        }
    }

    [BurstCompile]
    private void ApplyLightningEffect(SystemState state, EntityCommandBuffer ecb, ElementalReactionsConfig config)
    {
        //Apply Lightning
        foreach (var ( hitBuffer, _) 
                 in SystemAPI.Query<DynamicBuffer<HitBufferElement>, ElementalShouldApplyLightningComponent>())
        {
            foreach (var hit in hitBuffer)
            {
                if (hit.IsHandled) continue;
                if (!state.EntityManager.HasComponent<ElementalLightningEffectComponent>(hit.HitEntity))
                {
                    //Set Lightning
                    ecb.AddComponent<ElementalLightningEffectComponent>(hit.HitEntity);
                }
                else
                {
                    //Set Shock
                    int stacks = 0;
                    if (!state.EntityManager.HasComponent<ElementalShockEffectComponent>(hit.HitEntity))
                    {
                        ecb.AddComponent<ElementalShockEffectComponent>(hit.HitEntity);
                        stacks = 1;
                    }
                    else
                    {
                        stacks = state.EntityManager.GetComponentData<ElementalShockEffectComponent>(hit.HitEntity).Stacks +
                                 1;
                        if (stacks > config.MaxShockStacks) stacks = (int)config.MaxShockStacks;
                    }
                    
                    ecb.SetComponent(hit.HitEntity, new ElementalShockEffectComponent
                    {
                        Stacks = stacks,
                        HasBeenApplied = false,
                        CurrentDurationTime = 0,
                    });
                    
                    //Add Conduct Effect
                    if (state.EntityManager.HasComponent<ElementalIceEffectComponent>(hit.HitEntity))
                    {
                        ApplyConductEffect(state, hit, ecb, config);
                    }
                    
                    //Add combust effect
                    if (state.EntityManager.HasComponent<ElementalFireEffectComponent>(hit.HitEntity))
                    {
                        ApplyCombustionEffect(state, hit, ecb, config);
                    }
                    
                }
            }
        }
    }

    [BurstCompile]
    private void ApplyFireEffect(SystemState state, EntityCommandBuffer ecb, ElementalReactionsConfig config)
    {
                //Apply Fire
        foreach (var ( hitBuffer, _) 
                 in SystemAPI.Query<DynamicBuffer<HitBufferElement>, ElementalShouldApplyFireComponent>())
        {
            foreach (var hit in hitBuffer)
            {
                if (hit.IsHandled) continue;
                if (!state.EntityManager.HasComponent<ElementalFireEffectComponent>(hit.HitEntity))
                {
                    ecb.AddComponent<ElementalFireEffectComponent>(hit.HitEntity);
                }
                else
                {
                    //Add Burn effect
                    int stacks = 0;
                    if (!state.EntityManager.HasComponent<ElementalBurnEffectComponent>(hit.HitEntity))
                    {
                        ecb.AddComponent<ElementalBurnEffectComponent>(hit.HitEntity);
                        stacks = 1;
                    }
                    else
                    {
                        stacks = state.EntityManager.GetComponentData<ElementalBurnEffectComponent>(hit.HitEntity).Stacks +
                                 1;
                        if (stacks > config.MaxBurnStacks) stacks = (int)config.MaxBurnStacks;
                    }
                    
                    ecb.SetComponent(hit.HitEntity, new ElementalBurnEffectComponent
                    {
                        Stacks = stacks,
                        HasBeenApplied = false,
                        CurrentDurationTime = 0,
                    });

                    //apply vulnerable
                    if (state.EntityManager.HasComponent<ElementalIceEffectComponent>(hit.HitEntity))
                    {
                        ApplyVulnerableEffect(state, hit, ecb, config);
                    }
                    
                    //apply combustion
                    if (state.EntityManager.HasComponent<ElementalLightningEffectComponent>(hit.HitEntity))
                    {
                        ApplyCombustionEffect(state, hit, ecb, config);
                    }
                }
            }
        }
    }

    [BurstCompile]
    private void ApplyCombustionEffect(SystemState state, HitBufferElement hit, EntityCommandBuffer ecb,
        ElementalReactionsConfig config)
    {
        if (!state.EntityManager.HasComponent<ElementalCombustionEffectComponent>(hit.HitEntity))
        {
            ecb.AddComponent<ElementalCombustionEffectComponent>(hit.HitEntity);
        }
        else
        {
            return;
        }
                    
        ecb.SetComponent(hit.HitEntity, new ElementalCombustionEffectComponent
        {
            HasBeenApplied = false,
        });
    }

    [BurstCompile]
    private void ApplyConductEffect(SystemState state, HitBufferElement hit, EntityCommandBuffer ecb,
        ElementalReactionsConfig config)
    {
        int stacks = 0;
        if (!state.EntityManager.HasComponent<ElementalConductEffectComponent>(hit.HitEntity))
        {
            ecb.AddComponent<ElementalConductEffectComponent>(hit.HitEntity);
            stacks = 1;
        }
        else
        {
            stacks = state.EntityManager.GetComponentData<ElementalConductEffectComponent>(hit.HitEntity).Stacks +
                     1;
            if (stacks > config.MaxConductStacks) stacks = (int)config.MaxConductStacks;
        }
                    
        ecb.SetComponent(hit.HitEntity, new ElementalConductEffectComponent
        {
            Stacks = stacks,
            HasBeenApplied = false,
            CurrentDurationTime = 0,
        });
    }
    
    [BurstCompile]
    private void ApplyVulnerableEffect(SystemState state, HitBufferElement hit, EntityCommandBuffer ecb, ElementalReactionsConfig config)
    {
        //Add Vulnerable Effect

            int stacks = 0;
            if (!state.EntityManager.HasComponent<ElementalVulnerableEffectComponent>(hit.HitEntity))
            {
                ecb.AddComponent<ElementalVulnerableEffectComponent>(hit.HitEntity);
                stacks = 1;
            }
            else
            {
                stacks = state.EntityManager.GetComponentData<ElementalVulnerableEffectComponent>(hit.HitEntity).Stacks +
                         1;
                if (stacks > config.MaxVulnurableStacks) stacks = (int)config.MaxVulnurableStacks;
            }
                    
            ecb.SetComponent(hit.HitEntity, new ElementalVulnerableEffectComponent
            {
                Stacks = stacks,
                HasBeenApplied = false,
                CurrentDurationTime = 0,
            });
    }
    
}

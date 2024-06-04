using System.Collections;
using System.Collections.Generic;
using Damage;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

[UpdateBefore(typeof(AddDamageBufferElementOnCollisionSystem))]
[UpdateBefore(typeof(AddDamageBufferElementOnTriggerSystem))]
[UpdateInGroup(typeof(CombatSystemGroup))]
public partial struct AddElementalEffectSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ElementalVulnerableEffectConfig>();
        state.RequireForUpdate<ElementalCombustionEffectConfig>();
        state.RequireForUpdate<ElementalConductEffectConfig>();
        state.RequireForUpdate<ElementalShockEffectConfig>();
        state.RequireForUpdate<ElementalBurnConfig>();
        state.RequireForUpdate<ElementalFreezeEffectConfig>();
    }
        
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var freezeConfig = SystemAPI.GetSingleton<ElementalFreezeEffectConfig>();
        var burnConfig = SystemAPI.GetSingleton<ElementalBurnConfig>();
        var shockConfig = SystemAPI.GetSingleton<ElementalShockEffectConfig>();
        var conductConfig = SystemAPI.GetSingleton<ElementalConductEffectConfig>();
        var combustConfig = SystemAPI.GetSingleton<ElementalCombustionEffectConfig>();
        var vulnerableConfig = SystemAPI.GetSingleton<ElementalVulnerableEffectConfig>();
        int stacks = 0;
        
        
        ApplyFireEffect(state, ecb, burnConfig, vulnerableConfig, combustConfig);
        ApplyLightningEffect(state, ecb, shockConfig, conductConfig, combustConfig);
        ApplyIceEffect(state, ecb, freezeConfig, conductConfig, vulnerableConfig);
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    private void ApplyIceEffect(SystemState state, EntityCommandBuffer ecb, ElementalFreezeEffectConfig freezeConfig, ElementalConductEffectConfig conductConfig,
        ElementalVulnerableEffectConfig vulnerableConfig)
    {
        //Apply Ice
        foreach (var ( hitBuffer, _) 
                 in SystemAPI.Query<DynamicBuffer<HitBufferElement>, ElementalShouldApplyIceComponent>())
        {
            foreach (var hit in hitBuffer)
            {
                if (hit.IsHandled) continue;

                    //Add Freeze Effect
                    int stacks = 0;
                    if (!state.EntityManager.HasComponent<ElementalFreezeEffectComponent>(hit.HitEntity))
                    {
                        ecb.AddComponent<ElementalFreezeEffectComponent>(hit.HitEntity);
                        stacks = 1;
                        ecb.AddComponent<ShouldChangeMaterialComponent>(hit.HitEntity);
                        ecb.SetComponent(hit.HitEntity, new ShouldChangeMaterialComponent
                        {
                            MaterialType = MaterialType.FrozenMaterial,
                        });
                    }
                    else
                    {
                        stacks = state.EntityManager.GetComponentData<ElementalFreezeEffectComponent>(hit.HitEntity).Stacks +
                                 1;
                        if (stacks >= freezeConfig.MaxFreezeStacks) stacks = (int)freezeConfig.MaxFreezeStacks;
                    }
                    
                    ecb.SetComponent(hit.HitEntity, new ElementalFreezeEffectComponent
                    {
                        Stacks = stacks,
                        HasBeenApplied = false,
                        CurrentDurationTime = 0,
                    });
                    
                    // //Add Conduct Effect
                    // if (state.EntityManager.HasComponent<ElementalLightningEffectComponent>(hit.HitEntity))
                    // {
                    //     ApplyConductEffect(state, hit, ecb, conductConfig);
                    // }
                    //
                    // //Add Vulnerable Effect
                    // if (state.EntityManager.HasComponent<ElementalFireEffectComponent>(hit.HitEntity))
                    // {
                    //     ApplyVulnerableEffect(state, hit, ecb, vulnerableConfig);
                    // },mkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk$$$$$$$$$$$$$$$$$$$$$444444444oppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppp87
            }
        }
    }

    [BurstCompile]
    private void ApplyLightningEffect(SystemState state, EntityCommandBuffer ecb, ElementalShockEffectConfig shockConfig, ElementalConductEffectConfig conductConfig,
        ElementalCombustionEffectConfig combustConfig)
    {
        //Apply Lightning
        foreach (var ( hitBuffer, _) 
                 in SystemAPI.Query<DynamicBuffer<HitBufferElement>, ElementalShouldApplyLightningComponent>())
        {
            foreach (var hit in hitBuffer)
            {
                if (hit.IsHandled) continue;

                    //Set Shock
                    int stacks = 0;
                    if (!state.EntityManager.HasComponent<ElementalShockEffectComponent>(hit.HitEntity))
                    {
                        ecb.AddComponent<ElementalShockEffectComponent>(hit.HitEntity);
                        stacks = 1;
                        ecb.AddComponent<ShouldChangeMaterialComponent>(hit.HitEntity);
                        ecb.SetComponent(hit.HitEntity, new ShouldChangeMaterialComponent
                        {
                            MaterialType = MaterialType.ShockMaterial,
                        });
                    }
                    else
                    {
                        stacks = state.EntityManager.GetComponentData<ElementalShockEffectComponent>(hit.HitEntity).Stacks +
                                 1;
                        if (stacks > shockConfig.MaxShockStacks) stacks = (int)shockConfig.MaxShockStacks;
                    }
                    
                    ecb.SetComponent(hit.HitEntity, new ElementalShockEffectComponent
                    {
                        Stacks = stacks,
                        HasBeenApplied = false,
                        CurrentDurationTime = 0,
                    });
                    
                    // //Add Conduct Effect
                    // if (state.EntityManager.HasComponent<ElementalIceEffectComponent>(hit.HitEntity))
                    // {
                    //     ApplyConductEffect(state, hit, ecb, conductConfig);
                    // }
                    //
                    // //Add combust effect
                    // if (state.EntityManager.HasComponent<ElementalFireEffectComponent>(hit.HitEntity))
                    // {
                    //     ApplyCombustionEffect(state, hit, ecb, combustConfig);
                    // }
                    
                
            }
        }
    }

    [BurstCompile]
    private void ApplyFireEffect(SystemState state, EntityCommandBuffer ecb, ElementalBurnConfig burnConfig, ElementalVulnerableEffectConfig vulnerableConfig,
        ElementalCombustionEffectConfig combustConfig)
    {
                //Apply Fire
        foreach (var ( hitBuffer, _) 
                 in SystemAPI.Query<DynamicBuffer<HitBufferElement>, ElementalShouldApplyFireComponent>())
        {
            foreach (var hit in hitBuffer)
            {
                if (hit.IsHandled) continue;

                    //Add Burn effect
                    int stacks = 0;
                    if (!state.EntityManager.HasComponent<ElementalBurnEffectComponent>(hit.HitEntity))
                    {
                        ecb.AddComponent<ElementalBurnEffectComponent>(hit.HitEntity);
                        stacks = 1;
                        ecb.AddComponent<ShouldChangeMaterialComponent>(hit.HitEntity);
                        ecb.SetComponent(hit.HitEntity, new ShouldChangeMaterialComponent
                        {
                            MaterialType = MaterialType.BurnMaterial,
                        });
                    }
                    else
                    {
                        stacks = state.EntityManager.GetComponentData<ElementalBurnEffectComponent>(hit.HitEntity).Stacks +
                                 1;
                        if (stacks > burnConfig.MaxBurnStacks) stacks = (int)burnConfig.MaxBurnStacks;
                    }
                    
                    ecb.SetComponent(hit.HitEntity, new ElementalBurnEffectComponent
                    {
                        Stacks = stacks,
                        HasBeenApplied = false,
                        CurrentDurationTime = 0,
                    });

                    // //apply vulnerable
                    // if (state.EntityManager.HasComponent<ElementalIceEffectComponent>(hit.HitEntity))
                    // {
                    //     ApplyVulnerableEffect(state, hit, ecb, vulnerableConfig);
                    // }
                    //
                    // //apply combustion
                    // if (state.EntityManager.HasComponent<ElementalLightningEffectComponent>(hit.HitEntity))
                    // {
                    //     ApplyCombustionEffect(state, hit, ecb, combustConfig);
                    // }
                
            }
        }
    }

    [BurstCompile]
    private void ApplyCombustionEffect(SystemState state, HitBufferElement hit, EntityCommandBuffer ecb,
        ElementalCombustionEffectConfig config)
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
        ElementalConductEffectConfig config)
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
    private void ApplyVulnerableEffect(SystemState state, HitBufferElement hit, EntityCommandBuffer ecb, ElementalVulnerableEffectConfig config)
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

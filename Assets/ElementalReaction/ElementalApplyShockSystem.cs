using System.Collections;
using System.Collections.Generic;
using Damage;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(AddDamageBufferElementOnCollisionSystem))]
[UpdateBefore(typeof(AddDamageBufferElementOnTriggerSystem))]
[UpdateAfter(typeof(AddElementalEffectSystem))]
[UpdateInGroup(typeof(CombatSystemGroup))]
public partial struct ElementalApplyShockSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HammerSpecialConfig>();
        state.RequireForUpdate<ElementalShockEffectConfig>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<ElementalShockEffectConfig>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var buffer = SystemAPI.GetBuffer<HitBufferElement>(SystemAPI.GetSingletonEntity<ElementalShockEffectConfig>());
        
        
        foreach (var (shockComponent, affectedTransform, affectedEntity)
                 in SystemAPI.Query<RefRW<ElementalShockEffectComponent>, LocalTransform>().WithEntityAccess())
        {
            shockComponent.ValueRW.CurrentDurationTime += SystemAPI.Time.DeltaTime;
            
            if (shockComponent.ValueRO.HasBeenApplied)
            {
                ecb.RemoveComponent<ElementalShockEffectComponent>(affectedEntity);
                ecb.AddComponent<ShouldChangeMaterialComponent>(affectedEntity);
                ecb.SetComponent(affectedEntity, new ShouldChangeMaterialComponent
                {
                    MaterialType = MaterialType.BASEMATERIAL,
                });
                continue;
            }

            if (shockComponent.ValueRO.CurrentDurationTime >=
                config.ShockDelay && !shockComponent.ValueRO.HasBeenApplied)
            {

                var element = new HitBufferElement
                {
                    HitEntity = affectedEntity,
                    IsHandled = false,
                    Normal = new float3(1, 0, 0),
                    Position = affectedTransform.Position,
                };
                buffer.Add(element);
                
                var hammerConfig = SystemAPI.GetSingleton<HammerSpecialConfig>();
                var vfx = state.EntityManager.Instantiate(hammerConfig.HammerSparkPrefab);
                state.EntityManager.SetComponentData(vfx, new VisualEffectComponent
                {
                    EntityToFollow = affectedEntity,
                    ActiveTime = 1f,
                    ShouldFollowParentEntity = true,
                });
                
                shockComponent.ValueRW.HasBeenApplied = true;
            }
            
            
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

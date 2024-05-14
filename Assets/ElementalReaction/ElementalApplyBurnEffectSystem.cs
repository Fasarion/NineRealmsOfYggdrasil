using System.Collections;
using System.Collections.Generic;
using Damage;
using Health;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(AddDamageBufferElementOnCollisionSystem))]
[UpdateBefore(typeof(AddDamageBufferElementOnTriggerSystem))]
[UpdateAfter(typeof(AddElementalEffectSystem))]
[UpdateInGroup(typeof(CombatSystemGroup))]
public partial struct ElementalApplyBurnEffectSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ElementalBurnConfig>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<ElementalBurnConfig>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var buffer = SystemAPI.GetBuffer<HitBufferElement>(SystemAPI.GetSingletonEntity<ElementalBurnConfig>());
        
        foreach (var (burnComponent, affectedTransform, affectedEntity)
                 in SystemAPI.Query<RefRW<ElementalBurnEffectComponent>, LocalTransform>().WithEntityAccess())
        {
            burnComponent.ValueRW.CurrentDurationTime += SystemAPI.Time.DeltaTime;
            
            if (burnComponent.ValueRO.CurrentDurationTime > config.BurnDuration && burnComponent.ValueRO.HasBeenApplied)
            {
                ecb.RemoveComponent<ElementalBurnEffectComponent>(affectedEntity);
                ecb.AddComponent<ShouldChangeMaterialComponent>(affectedEntity);
                ecb.SetComponent(affectedEntity, new ShouldChangeMaterialComponent
                {
                    MaterialType = MaterialType.BASEMATERIAL,
                });
                continue;
            }

            if (burnComponent.ValueRO.CurrentDurationTime >=
                burnComponent.ValueRO.CurrentDamageCheckpoint * config.BurnDamageTicksTime)
            {
                burnComponent.ValueRW.CurrentDamageCheckpoint++;

                var element = new HitBufferElement
                {
                    HitEntity = affectedEntity,
                    IsHandled = false,
                    Normal = new float2(1, 0),
                    Position = affectedTransform.Position,
                };
                buffer.Add(element);
            }
                
            if (burnComponent.ValueRO.HasBeenApplied) continue;
            
            burnComponent.ValueRW.HasBeenApplied = true;
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

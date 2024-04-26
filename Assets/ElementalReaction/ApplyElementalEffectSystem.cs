using System.Collections;
using System.Collections.Generic;
using Damage;
using Movement;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateBefore(typeof(ApplyDamageSystem))]
[UpdateAfter(typeof(AddElementalEffectSystem))]
[UpdateInGroup(typeof(CombatSystemGroup))]
public partial struct ApplyElementalEffectSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ElementalReactionsConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<ElementalReactionsConfig>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
        foreach (var (freezeComponent, moveSpeedComponent, affectedEntity)
                 in SystemAPI.Query<RefRW<ElementalFreezeEffectComponent>, RefRW<MoveSpeedComponent>>().WithEntityAccess())
        {
            freezeComponent.ValueRW.CurrentDurationTime += SystemAPI.Time.DeltaTime;
            
            if (freezeComponent.ValueRO.CurrentDurationTime > config.FreezeDuration)
            {
                
                ecb.RemoveComponent<ElementalFreezeEffectComponent>(affectedEntity);
            }
                
            if (freezeComponent.ValueRO.HasBeenApplied) continue;
            
            
        }
    }
}

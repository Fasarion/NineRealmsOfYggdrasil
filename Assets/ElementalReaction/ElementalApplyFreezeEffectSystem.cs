using Damage;
using Movement;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[UpdateBefore(typeof(ApplyDamageSystem))]
[UpdateAfter(typeof(AddElementalEffectSystem))]
[UpdateInGroup(typeof(CombatSystemGroup))]
public partial struct ElementalApplyFreezeEffectSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ElementalFreezeEffectConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<ElementalFreezeEffectConfig>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
        foreach (var (freezeComponent, moveSpeedComponent, affectedEntity)
                 in SystemAPI.Query<RefRW<ElementalFreezeEffectComponent>, RefRW<MoveSpeedComponent>>().WithEntityAccess())
        {
            freezeComponent.ValueRW.CurrentDurationTime += SystemAPI.Time.DeltaTime;
            
            if (freezeComponent.ValueRO.CurrentDurationTime > config.FreezeDuration && freezeComponent.ValueRO.HasBeenApplied)
            {
                var stacks = freezeComponent.ValueRO.Stacks;
                if (stacks >= config.MaxFreezeStacks) stacks -= 1;
                var num = (math.pow(config.FreezeMovementSlowPercentage, stacks));
                
                moveSpeedComponent.ValueRW.Value /=
                    num;
                ecb.RemoveComponent<ElementalFreezeEffectComponent>(affectedEntity);
                
                ecb.AddComponent<ShouldChangeMaterialComponent>(affectedEntity);
                ecb.SetComponent(affectedEntity, new ShouldChangeMaterialComponent
                {
                    MaterialType = MaterialType.BASEMATERIAL,
                });
                continue;
            }
                
            if (freezeComponent.ValueRO.HasBeenApplied) continue;

            freezeComponent.ValueRW.HasBeenApplied = true;
            if (freezeComponent.ValueRO.Stacks >= config.MaxFreezeStacks) continue;
            moveSpeedComponent.ValueRW.Value *= (config.FreezeMovementSlowPercentage);
            

        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

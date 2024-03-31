using Health;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Damage
{
    [UpdateInGroup(typeof(CombatSystemGroup))]
    [UpdateAfter(typeof(ApplyDamageSystem))]
    [UpdateBefore(typeof(DisableHasChangedHealthTagsSystem))]
    public partial struct DamageFlashTimerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Enable Damage Timer for entities that has changed their HP
            foreach (var (hasChangedHP, entity) in SystemAPI
                .Query<HasChangedHP>()
                .WithEntityAccess())
            {
                Debug.Log("Enable flash!");

                // only applies for negative amounts
                //TODO: optimize with a new tag "HasTakenDamage"
                if (hasChangedHP.Amount >= 0)
                    continue;
                
                // activate flash timer
                if (SystemAPI.HasComponent<DamageFlashTimer>(entity))
                {
                    var timer = SystemAPI.GetComponent<DamageFlashTimer>(entity);
                    timer.CurrentTime = timer.FlashTime;
                    SystemAPI.SetComponent(entity, timer);
                    SystemAPI.SetComponentEnabled<DamageFlashTimer>(entity, true);
                }
                
            }
            
            // Update Damage Timer
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (color, damageFlashTimer, entity) in 
                SystemAPI.Query<RefRW<URPMaterialPropertyBaseColor>, RefRW<DamageFlashTimer>>()
                    .WithEntityAccess())
            {
                damageFlashTimer.ValueRW.CurrentTime -= deltaTime;
                
                // Reset Timer
                if (damageFlashTimer.ValueRO.CurrentTime <= 0f)
                {
                    color.ValueRW.Value = new float4(1, 1, 1, 1);
                    SystemAPI.SetComponentEnabled<DamageFlashTimer>(entity, false);
                    continue;
                }

                var percentage = damageFlashTimer.ValueRO.CurrentTime / damageFlashTimer.ValueRO.FlashTime;
                var value = 1 - percentage;

                color.ValueRW.Value = new float4(1, value, value, 1);
            }
        }
    }
}
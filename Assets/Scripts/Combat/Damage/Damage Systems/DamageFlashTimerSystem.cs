using System;
using Health;
using Player;
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
            float4 red = new float4(1f, 0.0f, 0.0f, 1f);;
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (currentColor, meshColor, damageFlashTimer, entity) in 
                SystemAPI.Query<RefRW<URPMaterialPropertyBaseColor>, MeshColor, RefRW<DamageFlashTimer>>()
                    .WithEntityAccess())
            {
                damageFlashTimer.ValueRW.CurrentTime -= deltaTime;

                var baseColor = meshColor.Value;
                
                // Reset Timer
                if (damageFlashTimer.ValueRO.CurrentTime <= 0f)
                {
                    currentColor.ValueRW.Value = baseColor;
                    SystemAPI.SetComponentEnabled<DamageFlashTimer>(entity, false);
                    continue;
                }

                var percentage = damageFlashTimer.ValueRO.CurrentTime / damageFlashTimer.ValueRO.FlashTime;
                var value = 1 - percentage;


                currentColor.ValueRW.Value = math.lerp(red, baseColor, value);
            }
        }
    }
}
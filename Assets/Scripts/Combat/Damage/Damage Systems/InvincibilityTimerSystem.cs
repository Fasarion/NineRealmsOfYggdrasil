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
    public partial struct InvincibilityTimerSystem : ISystem
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
                if (SystemAPI.HasComponent<InvincibilityComponent>(entity))
                {
                    var timer = SystemAPI.GetComponent<InvincibilityComponent>(entity);
                    timer.CurrentTime = timer.InvincibilityTime;
                    SystemAPI.SetComponent(entity, timer);
                    SystemAPI.SetComponentEnabled<InvincibilityComponent>(entity, true);
                }
                
            }

            // Update Invincibility Timer
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (invincibilityTimer, entity) in 
                SystemAPI.Query<RefRW<InvincibilityComponent>>()
                    .WithEntityAccess())
            {
                invincibilityTimer.ValueRW.CurrentTime -= deltaTime;
                
                // Reset Timer
                if (invincibilityTimer.ValueRO.CurrentTime <= 0f)
                {
                    SystemAPI.SetComponentEnabled<InvincibilityComponent>(entity, false);
                    continue;
                }
            }
        }
    }
}
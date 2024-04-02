using Damage;
using Destruction;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace Health
{
    [UpdateInGroup(typeof(CombatSystemGroup))]
    [UpdateBefore(typeof(DisableHasChangedHealthTagsSystem))]
    [UpdateBefore(typeof(ApplyDamageSystem))]
    public partial struct ChangeHpOnImpactSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            
            foreach (var (hpChangeOnImpact, currentHp, hitBuffer, entity) 
                in SystemAPI.Query<ChangeHpOnImpact, RefRW<CurrentHpComponent>, DynamicBuffer<HitBufferElement>>()
                    .WithNone<HasChangedHP>()
                    .WithEntityAccess())
            {
                float deltaHealth = 0;
                
                foreach (var hit in hitBuffer)
                {
                    if (hit.IsHandled) continue;

                    deltaHealth += hpChangeOnImpact.Value;
                }

                if (deltaHealth == 0) continue;
                
                // Tag entity with "HasChangedHP"
                SystemAPI.SetComponentEnabled<HasChangedHP>(entity, true);
                SystemAPI.SetComponent(entity, new HasChangedHP(deltaHealth));

                // Change health
                currentHp.ValueRW.Value += deltaHealth;
                
                // If zero health, mark entity with Destroy Tag so it is destroyed in a later system
                if (currentHp.ValueRO.Value <= 0)
                {
                    ecb.SetComponentEnabled<ShouldBeDestroyed>(entity, true);
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
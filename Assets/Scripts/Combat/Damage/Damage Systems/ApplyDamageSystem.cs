using Destruction;
using Health;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Damage
{
    [UpdateInGroup(typeof(DamageSystemGroup))]
    public partial struct ApplyDamageSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

            foreach (var (currentHP, damageBuffer, damageReceivingEntity) in SystemAPI
                .Query<RefRW<CurrentHpComponent>, DynamicBuffer<DamageBufferElement>>()
                .WithEntityAccess()
                .WithOptions(EntityQueryOptions.FilterWriteGroup))
            {
                float totalDamageToDeal = 0;

                // Add damage from all damage elements in Damage Element Buffer
                // TODO: Handle specific damage types, multiplier etc
                foreach (var damageElement in damageBuffer)
                {
                    totalDamageToDeal += damageElement.HitPoints;
                }
                
                // Clear damage buffer to avoid dealing damage multiple times an different frames
                damageBuffer.Clear();

                // Deal damage
                currentHP.ValueRW.Value -= totalDamageToDeal;

                // If zero health, mark entity with Destroy Tag so it is destroyed in a later system
                if (currentHP.ValueRO.Value <= 0)
                {
                    ecb.AddComponent<ShouldBeDestroyed>(damageReceivingEntity);
                }
            }
            
            // Play back all operations in entity command buffer
            ecb.Playback(state.EntityManager);
        }
    }
}
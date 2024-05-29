using Destruction;
using Health;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Damage
{
    [UpdateInGroup(typeof(CombatSystemGroup))]
    public partial struct ApplyDamageSystem : ISystem
    {
        private static readonly float CRITICAL_MODIFIER = 2;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RandomComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var randomComponent = SystemAPI.GetSingletonRW<RandomComponent>();
            var damageNumberBuffer = SystemAPI.GetSingletonBuffer<DamageNumberBufferElement>();

            foreach (var (currentHP, damageBuffer, damageReduction, damageReceivingTransform, damageReceivingEntity) in SystemAPI
                .Query<RefRW<CurrentHpComponent>, DynamicBuffer<DamageBufferElement>, DamageReductionComponent, LocalTransform>()
                .WithEntityAccess())
            {
                float totalDamageToDeal = 0;
                bool hasCrit = false;

                // Add damage from all damage elements in Damage Element Buffer
                foreach (var damageElement in damageBuffer)
                {
                    float randomFloat = randomComponent.ValueRW.random.NextFloat();
                    float damageToDeal = damageElement.DamageContents.DamageValue;

                    // Add critical damage
                    if (damageElement.DamageContents.CriticalRate > randomFloat)
                    {
                        damageToDeal *= CRITICAL_MODIFIER; //damageElement.DamageContents.CriticalModifier;
                        hasCrit = true;
                    }
                    
                    totalDamageToDeal += damageToDeal;
                    
                    //Apply damage reduction or increase
                    totalDamageToDeal *= damageReduction.Value;
                }
                
                // Clear damage buffer to avoid dealing damage multiple times an different frames
                damageBuffer.Clear();

                // Skip entity if no damage should be dealt
                if (totalDamageToDeal == 0)
                    continue;
                
                // mark entity as "HasChangedHP" with given HP change amount
                // TODO: add damage to amount instead of overriding it. Otherwise, this might cause problems if other systems
                // have changed the HP themselves (i.e. by healing)
                if (SystemAPI.HasComponent<HasChangedHP>(damageReceivingEntity))
                {
                    SystemAPI.SetComponentEnabled<HasChangedHP>(damageReceivingEntity, true);
                    SystemAPI.SetComponent(damageReceivingEntity, new HasChangedHP(-totalDamageToDeal));
                }

                // Deal damage
                currentHP.ValueRW.Value -= totalDamageToDeal;
                
                // Add damage info to damage number buffer
                var damageNumberElement = new DamageNumberBufferElement
                {
                    damage = totalDamageToDeal,
                    isCritical = hasCrit,
                    position = damageReceivingTransform.Position,
                };
                damageNumberBuffer.Add(damageNumberElement);
                

                // If zero health, mark entity with Destroy Tag so it is destroyed in a later system
                if (currentHP.ValueRO.Value <= 0)
                {
                    ecb.AddComponent<IsDyingComponent>(damageReceivingEntity);
                }
            }
            
            // Play back all operations in entity command buffer
            ecb.Playback(state.EntityManager);
        }
    }
}
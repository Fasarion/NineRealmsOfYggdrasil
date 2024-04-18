using Health;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace Damage
{
    /// <summary>
    /// System for adding Damage Buffer Elements to Damage Buffers. These Damage Buffers will be handled
    /// in later systems for applying damage to entities.
    /// </summary>
    [UpdateInGroup(typeof(CombatSystemGroup))]
    public partial struct AddDamageBufferElementOnTriggerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RandomComponent>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var damageBufferLookup = SystemAPI.GetBufferLookup<DamageBufferElement>();
            var random = SystemAPI.GetSingletonRW<RandomComponent>();
            
            foreach (var (hitBuffer, damageOnTrigger) in SystemAPI.Query<DynamicBuffer<HitBufferElement>, DamageOnTriggerComponent>())
            {
                foreach (var hit in hitBuffer)
                {
                    if (hit.IsHandled) continue;

                    var damageBuffer = damageBufferLookup[hit.HitEntity];

                   // float randomFloat = random.ValueRW.random.NextFloat();
                    
                    damageBuffer.Add(new DamageBufferElement
                    {
                        DamageContents = damageOnTrigger.Value
                    });
                }
            }
        }
    }
}
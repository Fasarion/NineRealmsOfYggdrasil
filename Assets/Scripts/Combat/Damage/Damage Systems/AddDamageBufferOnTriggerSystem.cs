using Health;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;

namespace Damage
{
    /// <summary>
    /// System for adding Damage Buffer Elements to Damage Buffers. These Damage Buffers will be handled
    /// in later systems for applying damage to entities.
    /// </summary>
    [UpdateInGroup(typeof(CombatSystemGroup))]
    public partial struct AddDamageBufferOnTriggerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var damageBufferLookup = SystemAPI.GetBufferLookup<DamageBufferElement>();
            
            foreach (var (hitBuffer, damageOnTrigger) in SystemAPI.Query<DynamicBuffer<HitBufferElement>, DamageOnTriggerComponent>())
            {
                foreach (var hit in hitBuffer)
                {
                    if (hit.IsHandled) continue;
                    var damageBuffer = damageBufferLookup[hit.HitEntity];
                    damageBuffer.Add(new DamageBufferElement
                    {
                        HitPoints = damageOnTrigger.DamageValue,
                        DamageType = damageOnTrigger.DamageType,
                    });
                }
            }
        }
    }
}
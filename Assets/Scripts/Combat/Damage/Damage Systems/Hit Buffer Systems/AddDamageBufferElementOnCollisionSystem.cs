using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;

namespace Damage
{
    [UpdateInGroup(typeof(CombatSystemGroup))]
    public partial struct AddDamageBufferElementOnCollisionSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var damageBufferLookup = SystemAPI.GetBufferLookup<DamageBufferElement>();
            
            foreach (var (hitBuffer, damageOnCollision) in 
                SystemAPI.Query<DynamicBuffer<HitBufferElement>, DamageOnCollisionComponent>())
            {
                foreach (var hit in hitBuffer)
                {
                    if (hit.IsHandled) continue;
                    
                    var damageBuffer = damageBufferLookup[hit.HitEntity];
                    damageBuffer.Add(new DamageBufferElement
                    {
                        HitPoints = damageOnCollision.Amount,
                    });
                }
            }
        } 
    }
}
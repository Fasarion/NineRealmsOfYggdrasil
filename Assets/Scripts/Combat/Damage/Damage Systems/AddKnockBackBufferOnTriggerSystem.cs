using Destruction;
using Health;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Damage
{
    [UpdateInGroup(typeof(CombatSystemGroup))]
    public partial struct AddKnockBackBufferOnTriggerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var knockBackBufferLookup = SystemAPI.GetBufferLookup<KnockBackBufferElement>();
            
            foreach (var (hitBuffer, knockBackComponent) in SystemAPI.Query<DynamicBuffer<HitBufferElement>, KnockBackForce>())
            {
                foreach (var hit in hitBuffer)
                {
                    if (hit.IsHandled) continue;
                    var knockBackBufferElements = knockBackBufferLookup[hit.HitEntity];
                    knockBackBufferElements.Add(new KnockBackBufferElement
                    {
                        KnockBackForce = hit.Normal * knockBackComponent.Value
                    });
                }
            }
        }
    }
}
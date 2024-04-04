using Destruction;
using Health;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Damage
{
    [UpdateInGroup(typeof(CombatSystemGroup))]
    public partial struct AddKnockBackBufferOnTriggerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerPositionSingleton>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>();
            var knockBackBufferLookup = SystemAPI.GetBufferLookup<KnockBackBufferElement>();
            
            foreach (var (transform, hitBuffer, knockBackComponent) 
                in SystemAPI.Query<LocalTransform, DynamicBuffer<HitBufferElement>, KnockBackForce>())
            {
                foreach (var hit in hitBuffer)
                {
                    if (hit.IsHandled) continue;
                    var knockBackBufferElements = knockBackBufferLookup[hit.HitEntity];

                    var forceDirection = knockBackComponent.KnockAwayFromPlayer ?
                        math.normalize(transform.Position - playerPos.Value).xz:
                        hit.Normal;

                    knockBackBufferElements.Add(new KnockBackBufferElement
                    {
                        KnockBackForce = forceDirection * knockBackComponent.Value
                    });
                }
            }
        }
    }
}
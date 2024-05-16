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
    public partial struct AddKnockBackOnHitSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerPositionSingleton>();
            state.RequireForUpdate<RandomComponent>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>();
            var knockBackBufferLookup = SystemAPI.GetBufferLookup<KnockBackBufferElement>();

            var random = SystemAPI.GetSingletonRW<RandomComponent>();
            
            foreach (var ( hitBuffer, knockBackComponent) 
                in SystemAPI.Query<DynamicBuffer<HitBufferElement>, KnockBackOnHitComponent>())
            {
                foreach (var hit in hitBuffer)
                {
                    if (hit.IsHandled) continue;
                    
                    // if (!SystemAPI.HasBuffer<KnockBackBufferElement>(hit.HitEntity))
                    // {
                    //     Debug.Log("Entity misses knock back element buffer!");
                    //
                    //     var transform = SystemAPI.GetComponent<LocalTransform>(hit.HitEntity);
                    //     Debug.Log($"Hit position: {transform.Position}");
                    // }
                    
                    var knockBackBufferElements = knockBackBufferLookup[hit.HitEntity];

                    float3 forceDirection = float3.zero;
                    switch (knockBackComponent.KnockDirection)
                    {
                        case KnockDirectionType.AlongHitNormal:
                            forceDirection = hit.Normal;
                            break;
                        
                        case KnockDirectionType.TowardsHitNormal:
                            forceDirection = -hit.Normal;
                            break;
                        
                        case KnockDirectionType.AwayFromPlayer:
                            forceDirection = math.normalize(hit.Position - playerPos.Value);
                            break;
                        
                        case KnockDirectionType.PerpendicularToPlayer:
                            var toPlayer = math.normalize(hit.Position - playerPos.Value);
                            
                            // half of the time knocks to the right, half the time knocks to the left
                            forceDirection = new float3(-toPlayer.y, 0, toPlayer.x);
                            if (random.ValueRW.random.NextFloat() > 0.5f)
                            {
                                forceDirection *= -1;
                            }
                            break;
                        
                    }

                    knockBackBufferElements.Add(new KnockBackBufferElement
                    {
                        KnockBackForce = forceDirection * knockBackComponent.Value
                    });
                }
            }
        }
    }
}
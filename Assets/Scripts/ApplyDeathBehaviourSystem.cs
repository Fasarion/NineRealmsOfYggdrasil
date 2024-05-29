using Damage;
using Destruction;
using Health;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(ApplyDamageSystem))]
[BurstCompile]
public partial struct ApplyDeathBehaviourSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerPositionSingleton>();
        state.RequireForUpdate<IsDyingComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var knockBackBufferLookup = SystemAPI.GetBufferLookup<KnockBackBufferElement>();
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>();
        
        foreach (var (currentHP, velocity, transform, dyingComponent, entity) in SystemAPI
                     .Query<CurrentHpComponent, RefRW<PhysicsVelocity>, LocalTransform, RefRW<IsDyingComponent>>()
                     .WithEntityAccess())
        {
            if (!dyingComponent.ValueRO.IsHandled)
            {
                //TODO: change animation
                //TODO: turn off movement??
                
                ecb.RemoveComponent<MoveTowardsPlayerComponent>(entity);
                var knockBackBufferElements = knockBackBufferLookup[entity];
                var forceDirection = math.normalize(transform.Position - playerPos.Value);
                var knockBackForce = currentHP.Value * -1;
                knockBackBufferElements.Add(new KnockBackBufferElement
                {
                    KnockBackForce = forceDirection * knockBackForce,
                });
                dyingComponent.ValueRW.IsHandled = true;
                
                continue;
            }

            if (math.length(velocity.ValueRO.Linear) <= 0)
            {
                ecb.SetComponentEnabled<ShouldBeDestroyed>(entity, true);
            }
        }
            
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

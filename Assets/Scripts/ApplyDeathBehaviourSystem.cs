using AI;
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

        
        bool audioBufferExists = SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<AudioBufferData> audioBuffer);

        if (audioBufferExists)
        {
            foreach (var (isDyingComponent, soundOnDeath) in SystemAPI
                .Query<IsDyingComponent, PlaySoundOnDeathComponent>())
            {
                if (isDyingComponent.IsHandled) continue;
                
                audioBuffer.Add(new AudioBufferData {AudioData = soundOnDeath.AudioData});
            }
        }


        foreach (var (_, entity) in SystemAPI
                     .Query<IsDyingComponent>()
                     .WithNone<EnemyTypeComponent>()
                     .WithEntityAccess())
        {
            ecb.SetComponentEnabled<ShouldBeDestroyed>(entity, true);
        }
        
        foreach (var (currentHP, mass, pDamping, velocity, transform, dyingComponent, entity) in SystemAPI
                     .Query<CurrentHpComponent, RefRW<PhysicsMass>, RefRW<PhysicsDamping>, RefRW<PhysicsVelocity>, LocalTransform, RefRW<IsDyingComponent>>()
                     .WithAll<EnemyTypeComponent>()
                     .WithEntityAccess())
        {
            if (!dyingComponent.ValueRO.IsHandled)
            {
                //TODO: change animation
                //TODO: turn off movement??
                
                ecb.RemoveComponent<MoveTowardsPlayerComponent>(entity);
                var knockBackBufferElements = knockBackBufferLookup[entity];
                var forceDirection = math.normalize(transform.Position - (playerPos.Value - new float3(0, 0.5f, 0)));
                var knockBackForce = currentHP.KillingBlowValue;
                var damping = 0.8f;
                pDamping.ValueRW.Linear = 1;
                mass.ValueRW.InverseMass = 0.001f;
                knockBackBufferElements.Add(new KnockBackBufferElement
                {
                    KnockBackForce = forceDirection * (knockBackForce * damping),
                });
                dyingComponent.ValueRW.IsHandled = true;
                
                continue;
            }

            if (math.length(velocity.ValueRO.Linear) <= 0.9f)
            {
                ecb.SetComponentEnabled<ShouldBeDestroyed>(entity, true);
            }
        }

        

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

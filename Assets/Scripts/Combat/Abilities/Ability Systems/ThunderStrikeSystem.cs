using System.Collections;
using System.Collections.Generic;
using AI;
using Damage;
using Destruction;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct ThunderStrikeSystem : ISystem
{
    private CollisionFilter _detectionFilter;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTargetingComponent>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<ThunderBoltConfig>();
        state.RequireForUpdate<ThunderStrikeAbility>();
        state.RequireForUpdate<ThunderStrikeConfig>();
        
        _detectionFilter = new CollisionFilter
        {
            BelongsTo = 1, // Projectile
            CollidesWith = 1 << 1 // Enemy
        };
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var thunderConfig = SystemAPI.GetSingleton<ThunderStrikeConfig>();
        var boltConfig = SystemAPI.GetSingleton<ThunderBoltConfig>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var enemiesBuffer = new DynamicBuffer<HitBufferElement>();
        var targetPositions = new NativeArray<float3>(thunderConfig.maxStrikes * 2, Allocator.Temp);
        var target = SystemAPI.GetSingletonEntity<PlayerTargetingComponent>();

        foreach (var (ability, timer, entity) in
                 SystemAPI.Query<RefRW<ThunderStrikeAbility>, RefRW<TimerObject>>()
                     .WithEntityAccess())
        {
            
            if (ability.ValueRO.strikeCounter >= thunderConfig.maxStrikes)
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);

            }

            
            if (!ability.ValueRO.isInitialized)
            {
                timer.ValueRW.maxTime = thunderConfig.MaxDurationTime;
                ability.ValueRW.isInitialized = true;
            }

            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;
            

            var currentCheckpointTime = thunderConfig.timeBetweenStrikes * ability.ValueRO.strikeCounter + 1;
            if (timer.ValueRO.currentTime > currentCheckpointTime)
            {
                ability.ValueRW.strikeCounter++;
                if (ability.ValueRO.strikeCounter > thunderConfig.maxStrikes)
                    ability.ValueRW.strikeCounter = thunderConfig.maxStrikes;
                
                var effect = state.EntityManager.Instantiate(boltConfig.abilityPrefab);

                float3 pos = state.EntityManager.GetComponentData<LocalTransform>(target).Position;
                
                Debug.Log($"pos: {pos}");
                
                //TODO: byt ut till target area
                int counter = 0;
                foreach (var transform in
                         SystemAPI.Query<LocalTransform>().WithAll<EnemyTypeComponent>())
                {
                    var targetPosition = transform.Position;
                    targetPositions[counter] = targetPosition;
                    counter++;
                    if (counter > thunderConfig.maxStrikes) break;
                }
                
                state.EntityManager.SetComponentData(effect, new LocalTransform
                {
                    Position = targetPositions[ability.ValueRO.strikeCounter] + new float3(0, 4, 0),
                    Rotation = Quaternion.Euler(-90f, 0f, 0f),
                    Scale = 1
                });
                state.EntityManager.SetComponentData(effect, new TimerObject
                {
                    maxTime = boltConfig.maxDisplayTime
                });
            }
        }

        foreach (var (ability, timer, transform, entity) in
                 SystemAPI.Query<RefRW<ThunderBoltAbility>, RefRW<TimerObject>, RefRW<LocalTransform>>()
                     .WithEntityAccess())
        {
            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;

            if (timer.ValueRO.currentTime > boltConfig.maxDisplayTime)
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);
            }
            
            if (ability.ValueRO.hasFired) continue;

            if (timer.ValueRO.currentTime >= boltConfig.damageDelayTime)
            {
                var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
                var hits = new NativeList<DistanceHit>(state.WorldUpdateAllocator);

                var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();

                float totalArea = boltConfig.maxArea;

                hits.Clear();
                
                //TODO: fixa smidigare...
                foreach (var (configEntity, hitBuffer) in
                         SystemAPI.Query<RefRW<ThunderBoltConfig>, DynamicBuffer<HitBufferElement>>())
                {

                    if (collisionWorld.OverlapSphere(transform.ValueRO.Position + new float3(0, -4, 0), totalArea,
                            ref hits, _detectionFilter))
                    {
                        foreach (var hit in hits)
                        {
                            var enemyPos = transformLookup[hit.Entity].Position;
                            var colPos = hit.Position;
                            float2 directionToHit = math.normalizesafe((enemyPos.xz - transform.ValueRO.Position.xz));

                            //Maybe TODO: kolla om hit redan finns i buffer
                            HitBufferElement element = new HitBufferElement
                            {
                                IsHandled = false,
                                HitEntity = hit.Entity,
                                Position = colPos,
                                Normal = directionToHit
                            };
                            hitBuffer.Add(element);
                        }
                    }

                    ability.ValueRW.hasFired = true;
                }
            }
        }

        ecb.Playback(state.EntityManager);
    }
}

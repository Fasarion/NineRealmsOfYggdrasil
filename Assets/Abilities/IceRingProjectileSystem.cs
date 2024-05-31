using System.Collections;
using System.Collections.Generic;
using Damage;
using Destruction;
using Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.PlayerLoop;

[BurstCompile]
[UpdateAfter(typeof(IceRingSystem))]
public partial struct IceRingProjectileSystem : ISystem
{
    private CollisionFilter _detectionFilter;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerPositionSingleton>();
        state.RequireForUpdate<IceRingProjectile>();

        _detectionFilter = new CollisionFilter
        {
            BelongsTo = 1, // Projectile
            CollidesWith = 1 << 1 | 1 << 5, // Enemy
        };
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingletonRW<IceRingConfig>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>();
        
         //spawned ability handling
        foreach (var (ability, timer, transform, entity) in
                 SystemAPI.Query<RefRW<IceRingProjectile>, RefRW<TimerObject>, RefRW<LocalTransform>>()
                     .WithEntityAccess())
        {
            //set up
            if (!ability.ValueRO.IsInitialized)
            {
                timer.ValueRW.maxTime = config.ValueRO.maxDisplayTime;
                ability.ValueRW.IsInitialized = true;
            }
            
            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;
            
            if (timer.ValueRO.currentTime >= timer.ValueRO.maxTime)
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);
            }

            if (ability.ValueRO.HasFired) continue;
            
            if (timer.ValueRO.currentTime >= config.ValueRO.damageDelayTime)
            {
                var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
                var hits = new NativeList<DistanceHit>(state.WorldUpdateAllocator);
                
                var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
        
                float totalArea = ability.ValueRO.Area;
                
                hits.Clear();

                var hitBuffer = state.EntityManager.GetBuffer<HitBufferElement>(entity);
                
                
                if (collisionWorld.OverlapSphere(playerPos.Value, totalArea, ref hits, _detectionFilter))
                {
                    foreach (var hit in hits)
                    {
                        var enemyPos = transformLookup[hit.Entity].Position;
                        var colPos = hit.Position;
                        float3 directionToHit = math.normalizesafe((enemyPos - transform.ValueRO.Position));

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
                ability.ValueRW.HasFired = true;
            }
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

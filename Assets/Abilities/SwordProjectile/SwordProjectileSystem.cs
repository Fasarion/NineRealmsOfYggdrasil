using System.Collections;
using System.Collections.Generic;
using Damage;
using Destruction;
using Health;
using Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct SwordProjectileSystem : ISystem
{
    private CollisionFilter _detectionFilter;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SwordComboAbilityConfig>();
        state.RequireForUpdate<PlayerPositionSingleton>();
        state.RequireForUpdate<SwordProjectile>();
        _detectionFilter = new CollisionFilter
        {
            BelongsTo = 1, // Projectile
            CollidesWith = 1 << 1 // Enemy
        };
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var buffer = SystemAPI.GetSingletonBuffer<SwordTrajectoryRecordingElement>();
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>();
        var config = SystemAPI.GetSingleton<SwordComboAbilityConfig>();
        
        foreach (var (projectile, transform, entity) in SystemAPI
                     .Query<RefRW<SwordProjectile>, RefRW<LocalTransform>>()
                     .WithEntityAccess())
        {
            float3 targetPosition = new float3(0, 0, 0);
            
            if (!projectile.ValueRO.HasTarget)
            { 
                var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
                var hits = new NativeList<DistanceHit>(state.WorldUpdateAllocator);

                float totalArea = config.Radius;

                float3 originPosition = playerPos.Value;

                hits.Clear();
                
                if (collisionWorld.OverlapSphere(originPosition, totalArea,
                        ref hits, _detectionFilter))
                {
                    foreach (var hit in hits)
                    {
                        if (state.EntityManager.HasComponent<SwordProjectileTarget>(hit.Entity)) continue;
                        
                        ecb.AddComponent<SwordProjectileTarget>(hit.Entity);
                        
                        targetPosition = hit.Position;
                        projectile.ValueRW.HasTarget = true;
                        break;
                    }
                }
            }

            if (!projectile.ValueRO.HasTarget)
            {
                var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
                var hits = new NativeList<DistanceHit>(state.WorldUpdateAllocator);

                float totalArea = config.Radius;

                float3 originPosition = playerPos.Value;

                hits.Clear();
                
                if (collisionWorld.OverlapSphere(originPosition, totalArea,
                        ref hits, _detectionFilter))
                {
                    foreach (var hit in hits)
                    {
                        targetPosition = hit.Position;
                        projectile.ValueRW.HasTarget = true;
                        break;
                    }
                }
            }

            if (!projectile.ValueRO.HasTarget)
            {
                targetPosition = new float3(0, 1000, 0);
                projectile.ValueRW.HasTarget = true;
            }
            
            if (projectile.ValueRO.CurrentTransformFrame >= projectile.ValueRO.BufferLength - 1)
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);
                continue;
            }

            transform.ValueRW.Position = playerPos.Value;

            if (!projectile.ValueRO.IsInitialized)
            {
                var directionToTarget = math.normalizesafe(playerPos.Value - targetPosition);
                projectile.ValueRW.BasePosition = targetPosition + directionToTarget * config.Offset;
                projectile.ValueRW.IsInitialized = true;
                ecb.SetComponentEnabled<GameObjectParticlePrefab>(entity, true);
                projectile.ValueRW.BaseRotation = quaternion.LookRotation(-directionToTarget, new float3(0,1,0));
            }
            quaternion combinedRotation = math.mul(projectile.ValueRO.BaseRotation, buffer[projectile.ValueRO.CurrentTransformFrame].Rotation);
            
            transform.ValueRW.Position = projectile.ValueRO.BasePosition + buffer[projectile.ValueRO.CurrentTransformFrame].Position;
            transform.ValueRW.Rotation = combinedRotation;
            
            projectile.ValueRW.CurrentTransformFrame++;
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

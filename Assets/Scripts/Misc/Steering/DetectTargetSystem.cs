using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct DetectTargetSystem : ISystem
{
    private CollisionFilter _detectionFilter;
        
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsWorldSingleton>();
        _detectionFilter = new CollisionFilter
        {
            BelongsTo = 1, // Projectile
            CollidesWith = 1 << 1 // Enemy
        };
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        var hits = new NativeList<DistanceHit>(state.WorldUpdateAllocator);
        
        foreach (var (targetSeeker, transform, entity) in SystemAPI.Query<RefRW<SeekTargetComponent>, LocalTransform>()
            .WithNone<HasSeekTargetEntity>()
            .WithEntityAccess())
        {
            hits.Clear();
            if (collisionWorld.OverlapSphere(transform.Position, targetSeeker.ValueRO.HalfMaxDistance, ref hits, _detectionFilter))
            {
                var closestDistance = float.MaxValue;
                var closestEntity = Entity.Null;

                foreach (var hit in hits)
                {
                    // skip last sought target
                    if (hit.Entity == targetSeeker.ValueRO.LastTargetEntity) continue;
                    
                    float distance = hit.Distance;
                    
                    // target is to close
                    if (distance < targetSeeker.ValueRO.MinDistanceForSeek) continue;
                    
                    float3 directionToHit = hit.Position - transform.Position;
                    float3 lookRotation = transform.Forward();

                    float maxAngle = targetSeeker.ValueRO.FovInRadians;
                    float dotProduct = math.dot(math.normalize(directionToHit), math.normalize(lookRotation));
                    float angle = math.acos(dotProduct);
                    
                    // outside FOV
                    if (angle > maxAngle) continue;

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEntity = hit.Entity;
                    }
                }

                // set seek target entity
                if (closestEntity != Entity.Null)
                {
                    state.EntityManager.SetComponentEnabled<HasSeekTargetEntity>(entity, true);
                    state.EntityManager.SetComponentData(entity, new HasSeekTargetEntity{TargetEntity = closestEntity});
                }
            }
        }
    }
}
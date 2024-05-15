using Movement;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct SteerToTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
            
        foreach (var (transform, direction, moveToTarget, hasTarget, entity) in 
            SystemAPI.Query<LocalTransform, RefRW<DirectionComponent>, RefRW<SeekTargetComponent>, HasSeekTargetEntity>()
                .WithEntityAccess())
        {
            // get entity transform - might not need the check
            if (transformLookup.TryGetComponent(hasTarget.TargetEntity, out var targetPosition))
            {
                var directionToTarget = targetPosition.Position - transform.Position;
                var distanceToTarget = math.distance(targetPosition.Position, transform.Position);
                
                float3 directionValue = math.normalizesafe(directionToTarget);

                // stop steering to entity if too close
                if (distanceToTarget < moveToTarget.ValueRO.MinDistanceAfterTargetFound)
                {
                    moveToTarget.ValueRW.LastTargetEntity = hasTarget.TargetEntity;
                    state.EntityManager.SetComponentEnabled<HasSeekTargetEntity>(entity, false);

                    // set y to 0 to remove entity going through the ground
                    directionValue.y = 0;
                    directionValue = math.normalizesafe(directionToTarget);
                }

                direction.ValueRW.Value = directionValue;
            }
        }
    }
}
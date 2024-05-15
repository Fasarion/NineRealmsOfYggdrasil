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
        var deltaTime = SystemAPI.Time.DeltaTime;
            
        foreach (var (transform, direction, moveToTarget, hasTarget, entity) in 
            SystemAPI.Query<LocalTransform, RefRW<DirectionComponent>, RefRW<SeekTargetComponent>, HasSeekTargetEntity>()
                .WithEntityAccess())
        {
            if (transformLookup.TryGetComponent(hasTarget.TargetEntity, out var targetPosition))
            {
                var directionToTarget = targetPosition.Position - transform.Position;
                var distanceToTarget = math.distance(targetPosition.Position, transform.Position);
                
                var directionValue = math.normalizesafe(directionToTarget);

                // stop steering to entity if too close
                if (distanceToTarget < moveToTarget.ValueRO.MinDistanceAfterTargetFound)
                {
                    moveToTarget.ValueRW.LastTargetEntity = hasTarget.TargetEntity;
                    state.EntityManager.SetComponentEnabled<HasSeekTargetEntity>(entity, false);
                    Debug.Log("Loose target");

                    directionValue.y = 0;
                }

                direction.ValueRW.Value = directionValue;
            }
        }
    }
}
using Movement;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct SteerToTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
        var deltaTime = SystemAPI.Time.DeltaTime;
            
        foreach (var (transform, direction, moveToTarget) in 
            SystemAPI.Query<LocalTransform, RefRW<DirectionComponent>, SeekTargetComponent>())
        {
            if (transformLookup.TryGetComponent(moveToTarget.TargetEntity, out var targetPosition))
            {
                //direction.ValueRW.Value = math.normalizesafe(targetPosition.Position - transform.Position);
                
                var directionValue = math.normalizesafe(targetPosition.Position - transform.Position);
                directionValue.y = 0;
                direction.ValueRW.Value = math.normalizesafe(directionValue);
            }
        }
    }
}
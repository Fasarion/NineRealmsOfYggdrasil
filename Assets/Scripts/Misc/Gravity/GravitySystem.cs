using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct GravitySystem : ISystem
{
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GravityComponent>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (transform, gravity, entity) in SystemAPI
            .Query<RefRW<LocalTransform>, GravityComponent>()
            .WithEntityAccess())
        {
            if (transform.ValueRO.Position.y < gravity.TargetYValue)
            {
                state.EntityManager.SetComponentEnabled<GravityComponent>(entity, false);
                continue;
            }

            transform.ValueRW.Position += new float3(0, -gravity.FallSpeed, 0) * SystemAPI.Time.DeltaTime;
        }
    }
}
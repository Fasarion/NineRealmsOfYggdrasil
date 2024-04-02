using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[UpdateAfter(typeof(SpawnSystem))]
[BurstCompile]
public partial struct EnemyMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnConfig>();
    }

    public void OnUpdate(ref SystemState state)
    {
        //state.Enabled = false;
        
        var config = SystemAPI.GetSingletonRW<SpawnConfig>();
        
        var query = SystemAPI.QueryBuilder().WithAll<TestEnemy, LocalTransform>().Build();
        
        var job = new CollisionMovementJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            target = config.ValueRO.centerPoint
        };
        //job.ScheduleParallel();
        
        

        // var otherHandle = LocalToWorldSystem.ComputeRootLocalToWorldJob
        
        //state.Dependency
       
        //JobHandle depends = default;
        JobHandle handle = job.ScheduleParallel(query, state.Dependency);
        handle.Complete();
    }
    
    [WithAll(typeof(TestEnemy))]
    [BurstCompile]
    public partial struct CollisionMovementJob : IJobEntity
    {
        public float DeltaTime;
        public float3 target;

        public void Execute(ref LocalTransform transform, ref TestEnemy testEnemy)
        {
            float3 direction = (target - transform.Position);
            float3 velocityVector = math.normalizesafe(direction * 5f);
            testEnemy.velocity = new float2(velocityVector.x, velocityVector.z);
            
            var newPosition = transform.Position +
                              new float3(testEnemy.velocity.x, 0, testEnemy.velocity.y) * DeltaTime;
            
            
            transform.Position = newPosition;
        }
    }
}

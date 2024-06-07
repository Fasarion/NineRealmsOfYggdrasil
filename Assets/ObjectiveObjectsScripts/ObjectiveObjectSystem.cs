using System.Collections;
using System.Collections.Generic;
using Destruction;
using Movement;
using Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(PlayerMovementSystem))]
[BurstCompile]
public partial struct ObjectiveObjectSystem : ISystem
{
    private JobHandle _checkDistanceJob;
    private JobHandle _moveObjectJob;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ObjectiveObjectConfig>();
        state.RequireForUpdate<PlayerPositionSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();
        
        var config = SystemAPI.GetSingleton<ObjectiveObjectConfig>();

        var playerPosition = SystemAPI.GetSingleton<PlayerPositionSingleton>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        _checkDistanceJob = new CheckObjectiveObjectDistanceJob
        {
            PlayerPosition = playerPosition.Value,
            ECB = ecb.AsParallelWriter(),
            DistanceRadiusSQ = config.BaseDistance * config.BaseDistance
        }.ScheduleParallel(new JobHandle());
        
        _checkDistanceJob.Complete();
        ecb.Playback(state.EntityManager);
        

        var ecb2 = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
        _moveObjectJob = new MoveObjectiveObjectJob()
        {
            PlayerPosition = playerPosition.Value,
            ECB = ecb2.AsParallelWriter(),
            MoveSpeed = config.MoveSpeed,
            DeltaTime = SystemAPI.Time.DeltaTime
            
        }.ScheduleParallel(_checkDistanceJob);
        
        _moveObjectJob.Complete();
        

        
        ecb2.Playback(state.EntityManager);
        
        state.Dependency.Complete();
        
        var buffer = SystemAPI.GetSingletonBuffer<ObjectivePickupBufferElement>();

        foreach (var (obj, _) in
                 SystemAPI.Query<ObjectiveObject, ShouldBeDestroyed>())
        {
            var element = new ObjectivePickupBufferElement{ Value = obj.type};
            buffer.Add(element);
        }
        
        ecb.Dispose();
        ecb2.Dispose();
        //state.Dependency.Complete();
    }
}

[WithAll(typeof(ObjectiveObject))]
[WithNone(typeof(DirectionComponent))]
[BurstCompile]
partial struct CheckObjectiveObjectDistanceJob : IJobEntity
{
    public float3 PlayerPosition;
    public EntityCommandBuffer.ParallelWriter ECB;
    public float DistanceRadiusSQ;

    void Execute(Entity entity, ref LocalTransform transform, [ChunkIndexInQuery] int chunkIndex)
    {
        if (math.distancesq(transform.Position, PlayerPosition) <= DistanceRadiusSQ)
        {
            ECB.SetComponentEnabled<DirectionComponent>(chunkIndex, entity, true);
        }
    }
}

[WithAll(typeof(ObjectiveObject))]
[WithAll(typeof(DirectionComponent))]
[BurstCompile]
partial struct MoveObjectiveObjectJob : IJobEntity
{
    public float3 PlayerPosition;
    public EntityCommandBuffer.ParallelWriter ECB;
    public float MoveSpeed;
    public float DeltaTime;

    void Execute(Entity entity, ref LocalTransform transform, [ChunkIndexInQuery] int chunkIndex)
    {
        var velocity = math.normalizesafe(PlayerPosition - transform.Position) * MoveSpeed;
        var newPos = transform.Position + (velocity * DeltaTime);
        transform.Position = newPos;
        
        
        if (math.distancesq(transform.Position, PlayerPosition) <= 1)
        {
            ECB.SetComponentEnabled<ShouldBeDestroyed>(chunkIndex, entity, true);
        }
        
    }
    
}



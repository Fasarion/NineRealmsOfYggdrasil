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
using UnityEngine.Serialization;

[UpdateAfter(typeof(PlayerMovement))]
[UpdateAfter(typeof(XPObjectSpawnSystem))]
[BurstCompile]
public partial struct XPObjectSystem : ISystem
{
    private JobHandle _checkDistanceJob;
    private JobHandle _moveObjectJob;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<PlayerXP>();
        state.RequireForUpdate<PlayerLevel>();
        state.RequireForUpdate<XPObjectConfig>();
        state.RequireForUpdate<PlayerPositionSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();
        
        var config = SystemAPI.GetSingleton<XPObjectConfig>();
        var level = SystemAPI.GetSingletonRW<PlayerLevel>();

        var playerPosition = SystemAPI.GetSingleton<PlayerPositionSingleton>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        //var ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        _checkDistanceJob = new CheckXPObjectDistanceJob
        {
            PlayerPosition = playerPosition.Value,
            //ECB = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            ECB = ecb.AsParallelWriter(),
            DistanceRadiusSQ = config.baseDistance * config.baseDistance
        }.ScheduleParallel(new JobHandle());
        
        _checkDistanceJob.Complete();
        ecb.Playback(state.EntityManager);
        

        var ecb2 = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
        _moveObjectJob = new MoveXPObjectJob
        {
            PlayerPosition = playerPosition.Value,
            //ECB = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            ECB = ecb2.AsParallelWriter(),
            MoveSpeed = config.moveSpeed,
            DeltaTime = SystemAPI.Time.DeltaTime
            
        }.ScheduleParallel(_checkDistanceJob);
        
        _moveObjectJob.Complete();
        
        ecb2.Playback(state.EntityManager);


        var query = SystemAPI.QueryBuilder().WithAll<ShouldBeDestroyed, XpObject>().Build();
        
        var queryCount = query.CalculateEntityCount();

        var destroyArray = query.ToEntityArray(Allocator.Temp);
        
        state.EntityManager.DestroyEntity(destroyArray);
        
        state.Dependency.Complete();
        
        
        
         var xp = SystemAPI.GetSingletonRW<PlayerXP>();
        
        var xpToAddPerObject = config.experience;
        var totalXpToAdd = 0;

        if (queryCount > 0)
        {
            for (int i = 0; i < queryCount; i++)
            {
                totalXpToAdd += xpToAddPerObject;
            }

            totalXpToAdd += xp.ValueRO.Value;
            xp.ValueRW.Value = totalXpToAdd;
        }
        
        
        //state.Dependency.Complete();
        

        
    }
    
    
}

[WithAll(typeof(XpObject))]
[WithNone(typeof(DirectionComponent))]
partial struct CheckXPObjectDistanceJob : IJobEntity
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

[WithAll(typeof(XpObject))]
[WithAll(typeof(DirectionComponent))]
[BurstCompile]
partial struct MoveXPObjectJob : IJobEntity
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
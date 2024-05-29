using Destruction;
using Movement;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PlayerMovement))]
[BurstCompile]
public partial struct XPObjectSystem : ISystem
{
    private JobHandle _checkDistanceJob;
    private JobHandle _moveObjectJob;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerXP>();
        state.RequireForUpdate<XPObjectConfig>();
        state.RequireForUpdate<PlayerPositionSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();
        
        var config = SystemAPI.GetSingleton<XPObjectConfig>();

        var playerPosition = SystemAPI.GetSingleton<PlayerPositionSingleton>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        _checkDistanceJob = new CheckXPObjectDistanceJob
        {
            PlayerPosition = playerPosition.Value,
            ECB = ecb.AsParallelWriter(),
            DistanceRadiusSQ = config.baseDistance * config.baseDistance,
            DeltaTime = SystemAPI.Time.DeltaTime,
        }.ScheduleParallel(new JobHandle());
        
        _checkDistanceJob.Complete();
        ecb.Playback(state.EntityManager);
        

        var ecb2 = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
        _moveObjectJob = new MoveXPObjectJob
        {
            PlayerPosition = playerPosition.Value,
            ECB = ecb2.AsParallelWriter(),
            MoveSpeed = config.moveSpeed,
            DeltaTime = SystemAPI.Time.DeltaTime
            
        }.ScheduleParallel(_checkDistanceJob);
        
        _moveObjectJob.Complete();
        
        ecb2.Playback(state.EntityManager);
        
        var totalXpToAdd = 0;

        
        foreach (var xpObject in
                 SystemAPI.Query<XpObject>()
                     .WithAll<ShouldBeDestroyed>())
        {
            totalXpToAdd += xpObject.XpAwardedOnPickup;
        }
        
        var xp = SystemAPI.GetSingletonRW<PlayerXP>();
        xp.ValueRW.XPValue += totalXpToAdd;
        
        //state.Dependency.Complete();
        
        ecb.Dispose();
        ecb2.Dispose();
    }
}

[WithAll(typeof(XpObject))]
[WithNone(typeof(DirectionComponent))]
[BurstCompile]
partial struct CheckXPObjectDistanceJob : IJobEntity
{
    public float3 PlayerPosition;
    public EntityCommandBuffer.ParallelWriter ECB;
    public float DistanceRadiusSQ;
    public float DeltaTime;

    void Execute(Entity entity, ref XpObject xpObject, ref LocalTransform transform, [ChunkIndexInQuery] int chunkIndex)
    {
        if (xpObject.TimerTime > xpObject.TimeBeforePickup)
        {
            if (math.distancesq(transform.Position, PlayerPosition) <= DistanceRadiusSQ)
            {
                ECB.SetComponentEnabled<DirectionComponent>(chunkIndex, entity, true);
            }
        }
        else
        {
            xpObject.TimerTime += DeltaTime;
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
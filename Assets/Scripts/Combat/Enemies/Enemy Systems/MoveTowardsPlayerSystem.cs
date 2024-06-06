using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Movement;
using Player;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Analytics;

public partial struct MoveTowardsPlayerSystem : ISystem
{
    private JobHandle _moveTowardsPlayerJobHandle;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerPositionSingleton>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();
        float3 playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value;
        float deltaTime = SystemAPI.Time.DeltaTime;

        _moveTowardsPlayerJobHandle = new MoveTowardsPlayerJob
        {
            playerPos = playerPos,
            deltaTime = deltaTime,
        }.ScheduleParallel(new JobHandle());

        _moveTowardsPlayerJobHandle.Complete();
    }
}

[WithAll(typeof(MoveTowardsPlayerComponent))]
[WithNone(typeof(HitStopComponent))]
[BurstCompile]
partial struct MoveTowardsPlayerJob : IJobEntity
{ 
    [Unity.Collections.ReadOnly] public float3 playerPos;
    public float deltaTime;
    
    
    public void Execute(ref LocalTransform transform, ref MoveSpeedComponent moveSpeedComponent, MoveTowardsPlayerComponent moveToPlayer)
    {
        float speed = moveSpeedComponent.Value;
            
        var distanceToPlayer = math.distancesq(playerPos, transform.Position);
        if (distanceToPlayer < moveToPlayer.MinimumDistanceForMovingSquared)
        {
            speed = -moveToPlayer.MoveAwayFromPlayerSpeed;
        }

        moveSpeedComponent.WasMoving = moveSpeedComponent.IsMoving;
        
        var direction = playerPos - transform.Position;
        direction.y = 0;
        quaternion lookRotation = math.normalizesafe(quaternion.LookRotation(direction, math.up()));
            
        transform.Rotation = lookRotation;

        //var lastPosition = transform.ValueRW.Position;
        transform.Position += math.normalize(direction) * speed * deltaTime;
            
        // const float distanceSquaredForStandingStill = 0.00001f;
        // bool moving = math.distancesq(lastPosition, transform.ValueRW.Position)  > distanceSquaredForStandingStill;
        moveSpeedComponent.IsMoving = math.abs(speed) > 0;
    }
}


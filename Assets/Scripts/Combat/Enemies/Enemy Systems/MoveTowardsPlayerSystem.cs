using System.Collections;
using System.Collections.Generic;
using Movement;
using Player;
using Unity.Burst;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Analytics;

public partial struct MoveTowardsPlayerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerPositionSingleton>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float3 playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value;
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        foreach (var (transform, moveSpeed, moveToPlayer) 
            in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MoveSpeedComponent>, MoveTowardsPlayerComponent>().WithNone<HitStopComponent>())
        {
            float speed = moveSpeed.ValueRO.Value;
            
            var distanceToPlayer = math.distancesq(playerPos, transform.ValueRO.Position);
            if (distanceToPlayer < moveToPlayer.MinimumDistanceForMovingSquared)
            {
                speed = -moveToPlayer.MoveAwayFromPlayerSpeed;
            }

            moveSpeed.ValueRW.WasMoving = moveSpeed.ValueRO.IsMoving;

            

            var direction = playerPos - transform.ValueRO.Position;
            direction.y = 0;
            quaternion lookRotation = math.normalizesafe(quaternion.LookRotation(direction, math.up()));
            
            transform.ValueRW.Rotation = lookRotation;

            //var lastPosition = transform.ValueRW.Position;
            transform.ValueRW.Position += math.normalize(direction) * speed * deltaTime;
            
            // const float distanceSquaredForStandingStill = 0.00001f;
            // bool moving = math.distancesq(lastPosition, transform.ValueRW.Position)  > distanceSquaredForStandingStill;
            moveSpeed.ValueRW.IsMoving = math.abs(speed) > 0;
        }
    }
}
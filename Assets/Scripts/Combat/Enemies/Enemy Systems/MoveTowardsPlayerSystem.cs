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

        float directionMultiplier = 1;

        foreach (var (transform, moveSpeed, moveToPlayer) 
            in SystemAPI.Query<RefRW<LocalTransform>, MoveSpeedComponent, MoveTowardsPlayerComponent>().WithNone<HitStopComponent>())
        {
            var distanceToPlayer = math.distancesq(playerPos, transform.ValueRO.Position);
            if (distanceToPlayer < moveToPlayer.MinimumDistanceForMovingSquared)
            {
                directionMultiplier = -1;
            }
            else
            {
                directionMultiplier = 1;
            }
            
            var direction = playerPos - transform.ValueRO.Position;
            direction.y = 0;
            quaternion lookRotation = math.normalizesafe(quaternion.LookRotation(direction, math.up()));
            
            transform.ValueRW.Rotation = lookRotation;
            transform.ValueRW.Position += math.normalize(direction) * moveSpeed.Value * deltaTime * directionMultiplier;
        }
    }
}
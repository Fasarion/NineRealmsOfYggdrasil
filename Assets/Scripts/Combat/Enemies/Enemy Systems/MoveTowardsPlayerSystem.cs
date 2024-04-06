using System.Collections;
using System.Collections.Generic;
using Movement;
using Player;
using Unity.Burst;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
            in SystemAPI.Query<RefRW<LocalTransform>, MoveSpeedComponent, MoveTowardsPlayerComponent>())
        {
            var direction = playerPos - transform.ValueRO.Position;
            direction.y = 0;
            quaternion lookRotation = math.normalizesafe(quaternion.LookRotation(direction, math.up()));
            
            transform.ValueRW.Rotation = lookRotation;
            transform.ValueRW.Position += math.normalize(direction) * moveSpeed.Value * deltaTime;
        }
    }
}

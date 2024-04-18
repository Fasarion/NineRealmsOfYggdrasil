using Movement;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct ShootPlayerWhenCloseSystem : ISystem
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

        foreach (var (transform, moveSpeed, moveToPlayer, shootWhenClose) 
            in SystemAPI.Query<RefRW<LocalTransform>, MoveSpeedComponent, MoveTowardsPlayerComponent, RefRW<ShootPlayerWhenCloseComponent>>())
        {
            shootWhenClose.ValueRW.CurrentCooldownTime += deltaTime;
            
            var distanceToPlayer = math.distancesq(playerPos, transform.ValueRO.Position);
            if (distanceToPlayer < moveToPlayer.MinimumDistanceForMoving)
            {
                if (shootWhenClose.ValueRO.CurrentCooldownTime > shootWhenClose.ValueRO.ShootingCooldownTime)
                {
                    Debug.Log("Shoot!");
                    shootWhenClose.ValueRW.CurrentCooldownTime = 0;
                }
                
                
                continue;
            }
        }
    }
}
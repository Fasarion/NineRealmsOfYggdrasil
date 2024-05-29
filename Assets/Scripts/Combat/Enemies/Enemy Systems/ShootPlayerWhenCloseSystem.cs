using Movement;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Weapon;

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
        
        foreach (var (transform, shootWhenClose, entity) 
            in SystemAPI.Query<LocalTransform, RefRW<AttackPlayerWhenCloseComponent>>()
                .WithNone<HitStopComponent>()
                .WithEntityAccess())
        {
            shootWhenClose.ValueRW.CurrentCooldownTime += deltaTime;
            
            var distanceToPlayer = math.distancesq(playerPos, transform.Position);
            if (distanceToPlayer <= shootWhenClose.ValueRO.MinimumDistanceForShootingSquared)
            {
                if (shootWhenClose.ValueRO.CurrentCooldownTime > shootWhenClose.ValueRO.ShootingCooldownTime)
                {
                    state.EntityManager.SetComponentEnabled<EnemyAttackAnimationComponent>(entity, true);
                    shootWhenClose.ValueRW.CurrentCooldownTime = 0;
                }
            }
        }
        
        // reset cooldown on hit
        foreach (var (transform, shootWhenClose, entity) 
            in SystemAPI.Query<LocalTransform, RefRW<AttackPlayerWhenCloseComponent>>()
                .WithAll<HitStopComponent>()
                .WithEntityAccess())
        {
            shootWhenClose.ValueRW.CurrentCooldownTime = 0;
        }
    }
}
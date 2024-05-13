
using Movement;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Weapon;

public partial struct BirdSpecialAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WeaponAttackCaller>();
        
        state.RequireForUpdate<PlayerRotationSingleton>();
        state.RequireForUpdate<PlayerPositionSingleton>();
        
        state.RequireForUpdate<PlayerTag>();
        
        state.RequireForUpdate<BirdsSpecialAttackConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingleton<WeaponAttackCaller>();
        var config = SystemAPI.GetSingletonRW<BirdsSpecialAttackConfig>();

        var chargeInfo = attackCaller.SpecialChargeInfo;
        if (chargeInfo.ChargingWeapon != WeaponType.Birds) return;
        
        var playerRot = SystemAPI.GetSingleton<PlayerRotationSingleton>();
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value;
        var playerPos2d = playerPos.xz;
        
        ChargeState currentChargeState = chargeInfo.chargeState;

        if (currentChargeState == ChargeState.Start)
        {
            // Spawn birds evenly space around player
            for (int i = 0; i < config.ValueRO.BirdCount; i++)
            {
                // Spawn projectiles (TODO: move to a general system, repeated code for this and bird special)
                foreach (var (transform, spawner, weapon, entity) in SystemAPI
                    .Query<LocalTransform, ProjectileSpawnerComponent, WeaponComponent>()
                    .WithAll<BirdsComponent>()
                    .WithEntityAccess())
                {
                    // instantiate bird
                    var birdProjectile = state.EntityManager.Instantiate(spawner.Projectile);
                    
                    // get spawn position
                    float angle = math.radians(config.ValueRO.AngleStep * i);
                    float x = playerPos.x + config.ValueRO.Radius * math.cos(angle);
                    float z = playerPos.z + config.ValueRO.Radius * math.sin(angle);
                    float3 spawnPosition = new float3(x, playerPos.y, z);
                    
                    // get rotation
                    float tangentAngle = angle + math.PI / 2f; // Adding PI/2 to get perpendicular tangent angle
                    quaternion rotation = quaternion.RotateY(tangentAngle); // Calculate the rotation quaternion based on the tangent angle

                    // update transform
                    var birdTransform = transform;
                    birdTransform.Rotation = rotation;
                    birdTransform.Position = spawnPosition;
                    state.EntityManager.SetComponentData(birdProjectile, birdTransform);

                    // set owner data
                    state.EntityManager.SetComponentData(birdProjectile, new HasOwnerWeapon
                    {
                        OwnerEntity = entity,
                        OwnerWasActive = weapon.InActiveState
                    });

                    // disable auto move
                    state.EntityManager.SetComponentEnabled<AutoMoveComponent>(birdProjectile, false);
                    
                    // set special movement
                    state.EntityManager.SetComponentEnabled<BirdSpecialMovementComponent>(birdProjectile, true);
                }
            }
        }
        
        if (currentChargeState == ChargeState.Ongoing)
        {
           // Debug.Log("Spin slowly...");
            
            // TODO: check for new charge level, increase speed
        }
        
        if (currentChargeState == ChargeState.Stop)
        {
            //Debug.Log("Release special!");
            
            // TODO: increase stats depending on released charge level
        }
    }
}

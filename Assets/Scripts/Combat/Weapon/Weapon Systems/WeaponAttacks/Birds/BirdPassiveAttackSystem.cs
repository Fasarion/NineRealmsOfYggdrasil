using Movement;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Weapon;

public partial struct BirdPassiveAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WeaponAttackCaller>();

        state.RequireForUpdate<PlayerRotationSingleton>();
        state.RequireForUpdate<PlayerPositionSingleton>();

        state.RequireForUpdate<PlayerTag>();

        state.RequireForUpdate<BirdPassiveAttackConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        bool shouldStartAttack = attackCaller.ValueRO.ShouldStartPassiveAttack(WeaponType.Birds);
        if (!shouldStartAttack) return;
        
        var playerRot = SystemAPI.GetSingleton<PlayerRotationSingleton>();
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value;
        var playerPos2d = playerPos.xz;

        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerLTW = SystemAPI.GetComponent<LocalToWorld>(playerEntity);
        var ltwMatrix = playerLTW.Value;

        var config = SystemAPI.GetSingletonRW<BirdPassiveAttackConfig>();
        int birdCount = config.ValueRO.BirdCount;
        
        // get targets
        
        // Spawn projectiles (TODO: move to a general system, repeated code for this and bird special)
        foreach (var (transform, spawner, weapon, entity) in SystemAPI
            .Query<LocalTransform, ProjectileSpawnerComponent, WeaponComponent>()
            .WithAll<BirdsComponent>()
            .WithEntityAccess())
        {
            float angleStep = 180f / birdCount;
            
            for (int i = 0; i < birdCount; i++)
            {
                float angleToRotate = 45f + angleStep * (i + 1);
                quaternion rotation = quaternion.RotateY(math.radians(angleToRotate)); // Convert angle to quaternion

                float3 forwardInLocalSpace = playerRot.Forward;//new float3(0, 0, 1);
                float3 forwardInGlobalSpace = math.rotate(rotation, forwardInLocalSpace); 

                
                // instantiate bird
                var birdProjectile = state.EntityManager.Instantiate(spawner.Projectile);

                // update transform
                var birdTransform = transform;
                birdTransform.Rotation = rotation;
                birdTransform.Position = playerPos;
                state.EntityManager.SetComponentData(birdProjectile, birdTransform);
            
                // set owner data
                state.EntityManager.SetComponentData(birdProjectile, new HasOwnerWeapon
                {
                    OwnerEntity = entity,
                    OwnerWasActive = weapon.InActiveState
                });
                
                // set direction
                state.EntityManager.SetComponentData(birdProjectile, new DirectionComponent()
                {
                    Value = forwardInGlobalSpace
                });
            }
        }
    }
}
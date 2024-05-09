using System.Collections;
using System.Collections.Generic;
using Movement;
using Patrik;
using Player;
using Unity.Burst;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Weapon;

[UpdateAfter(typeof(PlayerRotationSystem))]
public partial struct BirdNormalAttackSystem : ISystem
{
    private bool hasSpawned;
    private bool hasSetup;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WeaponAttackCaller>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

        bool isAttackingWithBirds = attackCaller.ValueRO.ActiveAttackData.IsAttacking &&
                                    attackCaller.ValueRO.ActiveAttackData.WeaponType == WeaponType.Birds;
        

        var playerRot = SystemAPI.GetSingleton<PlayerRotationSingleton>();
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value;
        bool shouldStartAttack = attackCaller.ValueRO.ShouldStartActiveAttack(WeaponType.Birds, AttackType.Normal);
        if (shouldStartAttack)
        {
            // Spawn projectiles
            foreach (var (  birdsComponent, transform, spawner, entity) in SystemAPI
                .Query<BirdsComponent, LocalTransform, ProjectileSpawnerComponent>()
                .WithEntityAccess())
            {
                for (int i = 0; i < 2; i++)
                {
                    // instantiate
                    var birdProjectile = state.EntityManager.Instantiate(spawner.Projectile);

                    float rotationAmount = 30;
                    if (i % 2 == 0)
                    {
                        rotationAmount *= -1;
                    }
                    
                    quaternion rotationQuaternion = quaternion.AxisAngle(new float3(0, 1, 0), math.radians(rotationAmount));
                    var direction = math.mul(rotationQuaternion, playerRot.Forward);
                    var directionQuaternion = math.mul(rotationQuaternion, playerRot.Value);
                    
                    var birdTransform = transform;
                    birdTransform.Rotation = directionQuaternion;
                    birdTransform.Position = playerPos;
                    
                    state.EntityManager.SetComponentData(birdProjectile, birdTransform);
                    state.EntityManager.SetComponentData(birdProjectile,
                        new DirectionComponent(math.normalizesafe(direction)));

                    // fetch owner data
                    var weapon = state.EntityManager.GetComponentData<WeaponComponent>(entity);

                    // set owner data
                    state.EntityManager.SetComponentData(birdProjectile, new HasOwnerWeapon
                    {
                        OwnerEntity = entity,
                        OwnerWasActive = weapon.InActiveState
                    });
                }
                
                hasSpawned = true;
            }
        }
    }
}

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
    private int lastIndex;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WeaponAttackCaller>();
        
        state.RequireForUpdate<PlayerRotationSingleton>();
        state.RequireForUpdate<PlayerPositionSingleton>();
        
        state.RequireForUpdate<PlayerTag>();
        
        state.RequireForUpdate<BirdsMovementSettingsComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

        bool shouldStartAttack = attackCaller.ValueRO.ShouldStartActiveAttack(WeaponType.Birds, AttackType.Normal);
        if (!shouldStartAttack) return;

        var playerRot = SystemAPI.GetSingleton<PlayerRotationSingleton>();
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value;
        var playerPos2d = playerPos.xz;

        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerLTW = SystemAPI.GetComponent<LocalToWorld>(playerEntity);
        var ltwMatrix = playerLTW.Value;

        // Spawn projectiles
        foreach (var (birdSettings, transform, spawner, entity) in SystemAPI
            .Query<BirdsMovementSettingsComponent, LocalTransform, ProjectileSpawnerComponent>()
            .WithEntityAccess())
        {
            // bool that is used to decide which point that the bird should go to first
            bool startWithPoint1 = lastIndex % 2 == 0;
            
            // instantiate bird
            var birdProjectile = state.EntityManager.Instantiate(spawner.Projectile);

            // update direction
            var direction = playerRot.Forward;
            state.EntityManager.SetComponentData(birdProjectile,
                new DirectionComponent(math.normalizesafe(direction)));
            
            // update transform
            var birdTransform = transform;
            birdTransform.Rotation = playerRot.Value;
            birdTransform.Position = playerPos;
            state.EntityManager.SetComponentData(birdProjectile, birdTransform);
            
            // get movement control points
            float4 localControlPoint1 = birdSettings.controlPoint1;
            float4 localControlPoint2 = birdSettings.controlPoint2;

            // transform controls points to world space
            var controlPoint1 = math.mul(ltwMatrix, localControlPoint1).xz; 
            var controlPoint2 = math.mul(ltwMatrix, localControlPoint2).xz;

            // set control points for bird
            BirdMovementComponent birdMovement = new BirdMovementComponent
            {
                initialDirection = direction,

                startPoint = playerPos2d,
                controlPoint1 = startWithPoint1 ? controlPoint1 : controlPoint2,
                controlPoint2 = startWithPoint1 ? controlPoint2 : controlPoint1,
                
                TimeToComplete = birdSettings.timeToCompleteMovement,
            };
            state.EntityManager.SetComponentData(birdProjectile, birdMovement);

            // fetch owner data
            var weapon = state.EntityManager.GetComponentData<WeaponComponent>(entity);

            // set owner data
            state.EntityManager.SetComponentData(birdProjectile, new HasOwnerWeapon
            {
                OwnerEntity = entity,
                OwnerWasActive = weapon.InActiveState
            });

            // update last index
            lastIndex = (lastIndex + 1) % 2;
        }
    }
}
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
        
        state.RequireForUpdate<BirdNormalAttackConfig>();
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
        
        var birdSettings = SystemAPI.GetSingleton<BirdNormalAttackConfig>(); 

        // Spawn projectiles (TODO: move to a general system, repeated code for this and bird special)
        foreach (var (transform, spawner, weapon, entity) in SystemAPI
            .Query<LocalTransform, ProjectileSpawnerComponent, WeaponComponent>()
            .WithAll<BirdsComponent>()
            .WithEntityAccess())
        {
            // instantiate bird
            var birdProjectile = state.EntityManager.Instantiate(spawner.Projectile);

            // update transform
            var birdTransform = transform;
            birdTransform.Rotation = playerRot.Value;
            birdTransform.Position = playerPos;
            state.EntityManager.SetComponentData(birdProjectile, birdTransform);
            
            // set owner data
            state.EntityManager.SetComponentData(birdProjectile, new HasOwnerWeapon
            {
                OwnerEntity = entity,
                OwnerWasActive = weapon.InActiveState
            });
            
            // disable auto move
            state.EntityManager.SetComponentEnabled<AutoMoveComponent>(birdProjectile, false);
            
            // get movement control points
            float4 localControlPoint1 = birdSettings.controlPoint1;
            float4 localControlPoint2 = birdSettings.controlPoint2;

            // transform controls points to world space
            var controlPoint1 = math.mul(ltwMatrix, localControlPoint1).xz; 
            var controlPoint2 = math.mul(ltwMatrix, localControlPoint2).xz;
            
            // bool that is used to decide which point that the bird should go to first
            bool startWithPoint1 = lastIndex % 2 == 0;

            // set control points for bird
            BirdNormalMovementComponent birdNormalMovement = new BirdNormalMovementComponent
            {
                startPoint = playerPos2d,
                controlPoint1 = startWithPoint1 ? controlPoint1 : controlPoint2,
                controlPoint2 = startWithPoint1 ? controlPoint2 : controlPoint1,
                
                TimeToComplete = birdSettings.timeToCompleteMovement,
            };
            state.EntityManager.SetComponentData(birdProjectile, birdNormalMovement);
            state.EntityManager.SetComponentEnabled<BirdNormalMovementComponent>(birdProjectile, true);
            
            // update last index
            lastIndex = (lastIndex + 1) % 2;
        }
    }
}
﻿
using Destruction;
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
    private int cachedChargeLevel;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WeaponAttackCaller>();
        
        state.RequireForUpdate<PlayerRotationSingleton>();
        state.RequireForUpdate<PlayerPositionSingleton>();
        
        state.RequireForUpdate<PlayerTag>();
        
        state.RequireForUpdate<BirdsSpecialAttackConfig>();
        state.RequireForUpdate<GameUnpaused>();
        
        cachedChargeLevel = -1;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingleton<WeaponAttackCaller>();
        var config = SystemAPI.GetSingletonRW<BirdsSpecialAttackConfig>();

        var chargeInfo = attackCaller.SpecialChargeInfo;
        if (chargeInfo.ChargingWeapon != WeaponType.Birds) return;
        
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value;

        if (config.ValueRO.InReleaseState)
        {
            config.ValueRW.lifeTimeTimer += SystemAPI.Time.DeltaTime;
            
            if (config.ValueRO.lifeTimeTimer > config.ValueRO.LifeTimeAfterRelease)
            {
                config.ValueRW.lifeTimeTimer = 0;
                config.ValueRW.InReleaseState = false;
                var attackCallerRW = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
                attackCallerRW.ValueRW.ReturnWeapon = true;

                config.ValueRW.CurrentRadius = config.ValueRO.InitialRadius;
            }

            return;
        }

        ChargeState currentChargeState = chargeInfo.chargeState;
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();

        // Start charge up
        if (currentChargeState == ChargeState.Start)
        {
            config.ValueRW.CenterPointEntity = playerEntity;
            
            // Spawn birds evenly spaced around player
            for (int i = 0; i < config.ValueRO.BirdCount; i++)
            {
                // Spawn projectiles (TODO: move to a general system, repeated code for this and bird special)
                foreach (var (spawner, weapon, entity) in SystemAPI
                    .Query<ProjectileSpawnerComponent, WeaponComponent>()
                    .WithAll<BirdsComponent>()
                    .WithEntityAccess())
                {
                    // instantiate bird
                    var birdProjectile = state.EntityManager.Instantiate(spawner.Projectile);
                    
                    float angle = math.radians(config.ValueRO.AngleStep * i);
                    
                    // set owner data
                    state.EntityManager.SetComponentData(birdProjectile, new HasOwnerWeapon
                    {
                        OwnerEntity = entity,
                        OwnerWasActive = weapon.InActiveState
                    });

                    // disable auto move
                    state.EntityManager.SetComponentEnabled<AutoMoveComponent>(birdProjectile, false);
                    
                    // set special movement
                    state.EntityManager.SetComponentEnabled<CircularMovementComponent>(birdProjectile, true);
                    state.EntityManager.SetComponentData(birdProjectile, new CircularMovementComponent
                    {
                        CurrentAngle = angle,
                        Radius = config.ValueRO.InitialRadius,
                        AngularSpeed = config.ValueRO.AngularSpeedDuringCharge,
                        BaseAngularSpeed = config.ValueRO.AngularSpeedDuringCharge,
                        CenterPointEntity = config.ValueRO.CenterPointEntity
                    });
                }
                
                config.ValueRW.HasReleased = false;
                config.ValueRW.InReleaseState = false;
            }
        }
        
        // During Charge Up
        if (currentChargeState == ChargeState.Ongoing)
        {
            // update radius
            float nextRadius = config.ValueRO.CurrentRadius +
                               config.ValueRO.RadiusIncreaseSpeed * SystemAPI.Time.DeltaTime;
            
            var currentRadius = math.min(nextRadius, config.ValueRO.TargetRadius);
            config.ValueRW.CurrentRadius = currentRadius;
            
            foreach (var (birdMovement,  entity) in SystemAPI
                .Query<RefRW<CircularMovementComponent>>()
                .WithEntityAccess())
            {
                birdMovement.ValueRW.Radius = currentRadius;
            }

            // On New Charge Level
            if (chargeInfo.Level > cachedChargeLevel)
            {
                cachedChargeLevel = chargeInfo.Level;

                var configEntity = SystemAPI.GetSingletonEntity<BirdsSpecialAttackConfig>();
                var speedBuffer = state.EntityManager.GetBuffer<AngularSpeedChargeStageBuffElement>(configEntity);

                float speedModifier = speedBuffer[cachedChargeLevel].Value.DuringChargeBuff;
            
                foreach (var (birdMovement,  entity) in SystemAPI
                    .Query<RefRW<CircularMovementComponent>>()
                    .WithEntityAccess())
                {
                    birdMovement.ValueRW.AngularSpeed = birdMovement.ValueRO.BaseAngularSpeed * speedModifier;
                }
            }
        }
        
        // Release Birds
        if (currentChargeState == ChargeState.Stop && !config.ValueRO.HasReleased)
        {
            cachedChargeLevel = chargeInfo.Level;

            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

            var configEntity = SystemAPI.GetSingletonEntity<BirdsSpecialAttackConfig>();
            var speedBuffer = state.EntityManager.GetBuffer<AngularSpeedChargeStageBuffElement>(configEntity);

            float speedModifier = speedBuffer[cachedChargeLevel].Value.AfterReleaseBuff;
            
            foreach (var (birdMovement,  entity) in SystemAPI
                .Query<RefRW<CircularMovementComponent>>()
                .WithEntityAccess())
            {
                birdMovement.ValueRW.BaseAngularSpeed = config.ValueRO.AngularSpeedAfterRelease;
                
                birdMovement.ValueRW.AngularSpeed = birdMovement.ValueRO.BaseAngularSpeed * speedModifier;
                
                ecb.AddComponent(entity, new DestroyAfterSecondsComponent
                {
                    TimeToDestroy = config.ValueRO.LifeTimeAfterRelease
                });
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            config = SystemAPI.GetSingletonRW<BirdsSpecialAttackConfig>();
            config.ValueRW.HasReleased = true;
            config.ValueRW.InReleaseState = true;
        }
    }
}

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

//[UpdateAfter(typeof(PlayerAttackSystem))]
[UpdateAfter(typeof(AttackStatTransferSystem))]
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
        var config = SystemAPI.GetSingletonRW<BirdsSpecialAttackConfig>();
        var configEntity = SystemAPI.GetSingletonEntity<BirdsSpecialAttackConfig>();
        if (!state.EntityManager.HasComponent<IsUnlocked>(configEntity)) return;
        var attackCaller = SystemAPI.GetSingleton<WeaponAttackCaller>();

        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);



        var chargeInfo = attackCaller.SpecialChargeInfo;
        if (chargeInfo.ChargingWeapon != WeaponType.Birds) return;
        
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
            
            var spawnCount = state.EntityManager.GetComponentData<SpawnCount>(configEntity);
            var layerCount = state.EntityManager.GetComponentData<SpawnCountMultiplier>(configEntity);
            
            config.ValueRW.BirdCount = spawnCount.Value;
            config.ValueRW.BirdLayers = layerCount.Value;

            for (int layer = 0; layer < config.ValueRW.BirdLayers; layer++)
            {
                float radius = config.ValueRO.InitialRadius + config.ValueRO.BirdLayersRadiusIncrease * layer;
                
                // Spawn birds evenly spaced around player
                for (int birdIndex = 0; birdIndex < config.ValueRO.BirdCount; birdIndex++)
                {
                    // Spawn projectiles (TODO: move to a general system, repeated code for this and bird special)
                    foreach (var (spawner, weapon, entity) in SystemAPI
                        .Query<ProjectileSpawnerComponent, WeaponComponent>()
                        .WithAll<BirdsComponent>()
                        .WithEntityAccess())
                    {
                        // instantiate bird
                        var birdProjectile = state.EntityManager.Instantiate(spawner.Projectile);

                        float angle = math.radians(config.ValueRO.AngleStep * birdIndex);
                        
                        // set owner data
                        state.EntityManager.SetComponentData(birdProjectile, new HasOwnerWeapon
                        {
                            OwnerEntity = entity,
                            OwnerWasActive = weapon.InActiveState
                        });

                        // disable auto move
                        state.EntityManager.SetComponentEnabled<AutoMoveComponent>(birdProjectile, false);

                        float angularSpeedDecrease = math.pow(config.ValueRO.SpeedChangePerLayer, layer);
                        
                        // set special movement
                        state.EntityManager.SetComponentEnabled<CircularMovementComponent>(birdProjectile, true);
                        state.EntityManager.SetComponentData(birdProjectile, new CircularMovementComponent
                        {
                            CurrentAngle = angle,
                            Radius = radius,
                            AngularSpeed = config.ValueRO.AngularSpeedDuringCharge * angularSpeedDecrease,
                            BaseAngularSpeed = config.ValueRO.AngularSpeedDuringCharge,
                            CenterPointEntity = config.ValueRO.CenterPointEntity,
                            Layer = layer,
                            CounterClockWiseMovement = layer % 2 == 0,
                        });

                        var cachedDamage = state.EntityManager.GetComponentData<CachedDamageComponent>(configEntity).Value.DamageValue;
                        
                        // update stats
                        ecb.AddComponent(birdProjectile, new UpdateStatsComponent
                        {
                            EntityToTransferStatsFrom = configEntity
                        });
                        
                    }
                    
                    
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
                birdMovement.ValueRW.Radius = currentRadius + birdMovement.ValueRO.Layer * config.ValueRO.BirdLayersRadiusIncrease;
            }

            // On New Charge Level
            if (chargeInfo.Level > cachedChargeLevel)
            {
                cachedChargeLevel = chargeInfo.Level;

                var speedBuffer = state.EntityManager.GetBuffer<AngularSpeedChargeStageBuffElement>(configEntity);

                float speedModifier = speedBuffer[cachedChargeLevel].Value.DuringChargeBuff;
                
                
            
                foreach (var (birdMovement,  entity) in SystemAPI
                    .Query<RefRW<CircularMovementComponent>>()
                    .WithEntityAccess())
                {
                    birdMovement.ValueRW.AngularSpeed = birdMovement.ValueRO.BaseAngularSpeed * speedModifier * math.pow(config.ValueRO.SpeedChangePerLayer, birdMovement.ValueRO.Layer);
                    
                    // update stats
                    ecb.AddComponent(entity, new UpdateStatsComponent
                    {
                        EntityToTransferStatsFrom = configEntity
                    });
                }
            }
        }
        
        // Release Birds
        if (currentChargeState == ChargeState.Stop && !config.ValueRO.HasReleased)
        {
            cachedChargeLevel = chargeInfo.Level;


            var speedBuffer = state.EntityManager.GetBuffer<AngularSpeedChargeStageBuffElement>(configEntity);

            float speedModifier = speedBuffer[cachedChargeLevel].Value.AfterReleaseBuff;
            
            // update damage based on buffer
            var chargeBuffer = state.EntityManager.GetBuffer<ChargeBuffElement>(configEntity);
            float damageMod = chargeBuffer[cachedChargeLevel].Value.DamageModifier;
            var cachedDamage = state.EntityManager.GetComponentData<CachedDamageComponent>(configEntity);
            cachedDamage.Value.DamageValue *= damageMod;
            state.EntityManager.SetComponentData(configEntity, cachedDamage);
            
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
                
                ecb.AddComponent(entity, new UpdateStatsComponent
                {
                    EntityToTransferStatsFrom = configEntity
                });
            }
            
            config = SystemAPI.GetSingletonRW<BirdsSpecialAttackConfig>();
            config.ValueRW.HasReleased = true;
            config.ValueRW.InReleaseState = true;
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
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

// TODO: add comments, remove magic variables
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
        var config = SystemAPI.GetSingletonRW<BirdPassiveAttackConfig>();
        var configEntity = SystemAPI.GetSingletonEntity<BirdPassiveAttackConfig>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        SeekTargetComponent configSeekTarget = SystemAPI.GetComponent<SeekTargetComponent>(configEntity);

        // go through active diving (passive) bird projectiles. 
        foreach (var (timer, entity) in SystemAPI
            .Query<RefRW<TimerObject>>()
            .WithAll<DiveMovementComponent, BirdProjectileComponent>()
            .WithNone<SeekTargetComponent>()
            .WithEntityAccess())
        {
            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;
            if (timer.ValueRO.currentTime > timer.ValueRO.maxTime)
            {
                SeekTargetComponent seekTargetComponent = configSeekTarget;
                
                ecb.AddComponent<SeekTargetComponent>(entity);
                ecb.SetComponent(entity, seekTargetComponent);

                HasSeekTargetEntity hasSeekTargetEntity = new HasSeekTargetEntity();
                
                // add has seek target component
                ecb.AddComponent<HasSeekTargetEntity>(entity);
                ecb.SetComponent(entity, hasSeekTargetEntity);
                ecb.SetComponentEnabled<HasSeekTargetEntity>(entity, false);
                
                // update stats
                ecb.AddComponent<UpdateStatsComponent>(entity);
                UpdateStatsComponent updateStatsComponent = new UpdateStatsComponent
                    {EntityToTransferStatsFrom = configEntity};
                ecb.SetComponent(entity, updateStatsComponent);
            }
        }
        
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        bool shouldStartAttack = attackCaller.ValueRO.ShouldStartPassiveAttack(WeaponType.Birds);
        if (shouldStartAttack)
        {
            var playerRot = SystemAPI.GetSingleton<PlayerRotationSingleton>();
            var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value;
            
            int birdCount = config.ValueRO.BirdCount;
            
            
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
                    quaternion rotation = quaternion.RotateY(math.radians(angleToRotate));

                    float3 forwardInLocalSpace = playerRot.Forward;
                    float3 forwardInGlobalSpace = math.rotate(rotation, forwardInLocalSpace); 
                        
                    // instantiate bird
                    var birdProjectile = state.EntityManager.Instantiate(spawner.Projectile);

                    // update transform
                    var birdTransform = transform;
                    birdTransform.Rotation = rotation;
                    birdTransform.Position = playerPos + new float3(0, config.ValueRO.SpawnHeight, 0);
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
                        
                        
                    // add self destruct after seconds
                    ecb.AddComponent<DestroyAfterSecondsComponent>(birdProjectile);
                    ecb.SetComponent(birdProjectile, new DestroyAfterSecondsComponent{TimeToDestroy = config.ValueRO.LifeTime});

                    // adding a timer for a seek delay
                    ecb.AddComponent<TimerObject>(birdProjectile);
                    ecb.SetComponent(birdProjectile, new TimerObject{maxTime = config.ValueRO.SeekDelay});
                        
                    ecb.SetComponentEnabled<DiveMovementComponent>(birdProjectile, true);
                }
            }
                
        }
        
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
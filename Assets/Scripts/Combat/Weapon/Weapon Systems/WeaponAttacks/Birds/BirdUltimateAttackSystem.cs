using Damage;
using Destruction;
using Movement;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Weapon;


[BurstCompile]
[UpdateAfter(typeof(AttackStatTransferSystem))]
[UpdateAfter(typeof(PlayerRotationSystem))]
[UpdateAfter(typeof(CombatStatHandleSystem))]
public partial struct BirdUltimateAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MousePositionComponent>();
        state.RequireForUpdate<MousePositionInput>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<BirdsUltimateAttackConfig>();
        state.RequireForUpdate<GameUnpaused>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        var config = SystemAPI.GetSingletonRW<BirdsUltimateAttackConfig>();
        var configEntity = SystemAPI.GetSingletonEntity<BirdsUltimateAttackConfig>();

        var tornadoSpawner = state.EntityManager.GetComponentData<BirdnadoSpawnerComponent>(configEntity);

        // if active
        if (config.ValueRO.IsActive)
        {
            var targetPos = SystemAPI.GetComponent<LocalTransform>(config.ValueRO.CenterPointEntity).Position +
                            tornadoSpawner.TornadoOffset;
            if (config.ValueRO.UseMouse)
            {
                var mousePos = SystemAPI.GetSingleton<MousePositionInput>();
                targetPos = mousePos.WorldPosition + tornadoSpawner.TornadoOffset;
            }

            foreach (var (transform, timer, hitBuffer) in SystemAPI
                .Query<RefRW<LocalTransform>, RefRW<TimerObject>, DynamicBuffer<HitBufferElement>>()
                .WithAll<BirdnadoComponent, PartOfBirdUltimate>())
            {
                transform.ValueRW.Position = targetPos;
            }
            
            
            config.ValueRW.LifeTimeTimer += SystemAPI.Time.DeltaTime;
            
            var specialInput = SystemAPI.GetSingleton<PlayerSpecialAttackInput>();
            var normalInput = SystemAPI.GetSingleton<PlayerNormalAttackInput>();
            
            bool cancelInoutPressed = specialInput.KeyDown || normalInput.KeyDown;
            bool lifeTimeComplete = config.ValueRO.LifeTimeTimer > config.ValueRO.LifeTime;

            bool shouldReturn = cancelInoutPressed || lifeTimeComplete;
            
            if (shouldReturn)
            {
                var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
                
                // destroy all birds on return
                foreach (var (_, entity) in SystemAPI
                    .Query<BirdProjectileComponent>()
                    .WithAll<CircularMovementComponent>()
                    .WithEntityAccess())
                {
                    ecb.AddComponent<ShouldBeDestroyed>(entity);
                }
                
                foreach (var (_, entity) in SystemAPI
                    .Query<BirdnadoComponent>()
                    .WithAll<PartOfBirdUltimate>()
                    .WithEntityAccess())
                {
                    ecb.AddComponent<ShouldBeDestroyed>(entity);
                }
                
                config.ValueRW.LifeTimeTimer = 0;
                config.ValueRW.IsActive = false;
                
                // inform animator to finish ult animation
                var attackCallerRW = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
                attackCallerRW.ValueRW.ReturnWeapon = true;
                
                ecb.Playback(state.EntityManager);
                ecb.Dispose();
            }

            return;
        }
        
        bool startAttack = attackCaller.ValueRO.ShouldStartActiveAttack(WeaponType.Birds, AttackType.Ultimate);
        if (startAttack)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

            config.ValueRW.IsActive = true;
            
            var mousePos = SystemAPI.GetSingleton<MousePositionInput>().WorldPosition;
            
            config.ValueRW.CenterPointEntity = SystemAPI.GetSingletonEntity<MousePositionComponent>();

            var spawnCount = state.EntityManager.GetComponentData<SpawnCount>(configEntity);

            config.ValueRW.BirdCount = spawnCount.Value;
            if (state.EntityManager.HasComponent<UseMousePosition>(configEntity))
            {
                config.ValueRW.UseMouse = true;
            }
            
            
            state.EntityManager.SetComponentEnabled<ShouldSpawnBirdnado>(configEntity, true);

            // // spawn tornado
            // var tornado = state.EntityManager.Instantiate(tornadoSpawner.TornadoPrefab);
            //
            // // set size of tornado as configs diameter
            // var tornadoTransform = state.EntityManager.GetComponentData<LocalTransform>(tornado);
            // tornadoTransform.Scale = tornadoSpawner.TornadoRadius * 2;
            // state.EntityManager.SetComponentData(tornado, tornadoTransform);
            //
            // // set tornado suction rate
            // state.EntityManager.SetComponentData(tornado, new TimerObject{maxTime = tornadoSpawner.TimeBetweenSuctions});
            //
            // // play sound
            // var audioBuffer = SystemAPI.GetSingletonBuffer<AudioBufferData>();
            // audioBuffer.Add(new AudioBufferData { AudioData = config.ValueRO.TornadoSound});
            //
            //
            // // set damage
            // CachedDamageComponent thisDamage = state.EntityManager.GetComponentData<CachedDamageComponent>(configEntity);
            // state.EntityManager.SetComponentData(tornado, thisDamage);

            // Spawn birds evenly spaced around player
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
                    
                    // // get spawn position
                    float angle = math.radians(config.ValueRO.AngleStep * i);
                    float x = mousePos.x + config.ValueRO.Radius * math.cos(angle);
                    float z = mousePos.z + config.ValueRO.Radius * math.sin(angle);
                    float3 spawnPosition = new float3(x, 0, z);
                    
                    // get rotation
                    quaternion rotation = quaternion.RotateY(-angle); 
                    
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
                    
                    // set movement
                    state.EntityManager.SetComponentEnabled<CircularMovementComponent>(birdProjectile, true);
                    state.EntityManager.SetComponentData(birdProjectile, new CircularMovementComponent
                    {
                        CurrentAngle = angle,
                        Radius = config.ValueRO.Radius,
                        AngularSpeed = config.ValueRO.AngularSpeed,
                        BaseAngularSpeed = config.ValueRO.AngularSpeed,
                        CenterPointEntity = config.ValueRO.CenterPointEntity,
                    });
                    
                    // disable auto move
                    state.EntityManager.SetComponentEnabled<AutoMoveComponent>(birdProjectile, false);
                    
                    // update stats
                    ecb.AddComponent<UpdateStatsComponent>(birdProjectile);
                    UpdateStatsComponent updateStatsComponent = new UpdateStatsComponent
                        {EntityToTransferStatsFrom = configEntity};
                    ecb.SetComponent(birdProjectile, updateStatsComponent);
                    
                    // remove energy fill
                    if (state.EntityManager.HasComponent<EnergyFillComponent>(birdProjectile))
                    {
                        ecb.RemoveComponent<EnergyFillComponent>(birdProjectile);
                    }
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
        
        
    }
}
using Damage;
using Destruction;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Weapon;


public partial struct BirdUltimateAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<BirdsUltimateAttackConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        var config = SystemAPI.GetSingletonRW<BirdsUltimateAttackConfig>();

        // if active
        if (config.ValueRO.IsActive)
        {
            foreach (var (transform, timer, hitBuffer) in SystemAPI
                .Query<RefRW<LocalTransform>, RefRW<TimerObject>, DynamicBuffer<HitBufferElement>>()
                .WithAll<BirdnadoComponent>())
            {
                transform.ValueRW.Position =
                    SystemAPI.GetComponent<LocalTransform>(config.ValueRO.CenterPointEntity).Position;

                timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;
                
                // clears hitbuffer based on timer
                if (timer.ValueRO.currentTime > timer.ValueRO.maxTime)
                {
                    hitBuffer.Clear();
                    timer.ValueRW.currentTime = 0;
                }
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
                    .WithEntityAccess())
                {
                    ecb.AddComponent<ShouldBeDestroyed>(entity);
                }
                
                // destroy birdnado on return
                foreach (var (_, entity) in SystemAPI
                    .Query<BirdnadoComponent>()
                    .WithEntityAccess())
                {
                    ecb.AddComponent<ShouldBeDestroyed>(entity);
                }
                
                config.ValueRW.LifeTimeTimer = 0;
                config.ValueRW.IsActive = false;
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
            config.ValueRW.IsActive = true;
            
            var mousePos = SystemAPI.GetSingleton<MousePositionInput>().WorldPosition;
            var configRO = SystemAPI.GetSingleton<BirdsUltimateAttackConfig>();

            config.ValueRW.CenterPointEntity = SystemAPI.GetSingletonEntity<MousePositionComponent>();

            // spawn tornado
            var tornado = state.EntityManager.Instantiate(config.ValueRO.TornadoPrefab);
            
            // set size of tornado as configs diameter
            var tornadoTransform = state.EntityManager.GetComponentData<LocalTransform>(tornado);
            tornadoTransform.Scale = config.ValueRO.Radius * 2;
            state.EntityManager.SetComponentData(tornado, tornadoTransform);

            // Spawn birds evenly spaced around player
            for (int i = 0; i < configRO.BirdCount; i++)
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
                    float angle = math.radians(configRO.AngleStep * i);
                    float x = mousePos.x + configRO.Radius * math.cos(angle);
                    float z = mousePos.z + configRO.Radius * math.sin(angle);
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
                        Radius = configRO.Radius,
                        AngularSpeed = configRO.AngularSpeed,
                        BaseAngularSpeed = configRO.AngularSpeed,
                        CenterPointEntity = config.ValueRO.CenterPointEntity,
                    });
                }
            }
        }
    }
}
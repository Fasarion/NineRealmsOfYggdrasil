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
                        moveAroundType = CircularMovementComponent.MoveAroundType.Mouse
                    });
                }
            }
        }

        //attackCaller.ValueRW.ActiveAttackData.ShouldStart = false;
    }
}
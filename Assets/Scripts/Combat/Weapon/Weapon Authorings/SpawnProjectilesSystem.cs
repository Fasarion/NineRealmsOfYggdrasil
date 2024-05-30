using Movement;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Weapon;

public partial struct SpawnProjectilesSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        bool bufferExists = SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<AudioBufferData> audioBuffer);

        if (bufferExists)
        {
            var random = SystemAPI.GetSingletonRW<RandomComponent>();
        
            foreach (var playSound in SystemAPI.Query<PlaySoundOnProjectileFireComponent>()
                .WithAll<ShouldSpawnProjectile>())
            {
                float randomFloat = random.ValueRW.random.NextFloat();
            
                if (randomFloat > playSound.ChanceToPlay)
                {
                    audioBuffer.Add(new AudioBufferData {AudioData = playSound.AudioData});
                }
            }
        }
        
        // Spawn projectile
        foreach (var (  spawnerTransform, projectileSpawner, spawnerEntity) 
            in SystemAPI.Query< LocalTransform, ProjectileSpawnerComponent>()
                .WithAll<ShouldSpawnProjectile>()
                .WithEntityAccess())
        {
            Entity projectileEntity = entityManager.Instantiate(projectileSpawner.Projectile);
            var projectileTransform = entityManager.GetComponentData<LocalTransform>(projectileEntity);

            bool isWeapon = entityManager.HasComponent<WeaponComponent>(spawnerEntity);
            if (isWeapon)
            {
                var weapon = entityManager.GetComponentData<WeaponComponent>(spawnerEntity);
                
                projectileTransform.Position = weapon.AttackPoint.Position;
                projectileTransform.Rotation = math.mul(weapon.AttackPoint.Rotation, projectileTransform.Rotation);
                
                if (entityManager.HasComponent<HasOwnerWeapon>(projectileEntity))
                {
                    // set owner data
                    entityManager.SetComponentData(projectileEntity, new HasOwnerWeapon
                    {
                        OwnerEntity = spawnerEntity,
                        OwnerWasActive = weapon.InActiveState
                    });
                }
            }
            else
            {
                projectileTransform.Position = spawnerTransform.Position;
                projectileTransform.Rotation = math.mul(spawnerTransform.Rotation, projectileTransform.Rotation);
            }

            if (entityManager.HasComponent<DirectionComponent>(projectileEntity))
            {
                // set new transform values and direction
                entityManager.SetComponentData(projectileEntity, projectileTransform);
                entityManager.SetComponentData(projectileEntity, new DirectionComponent(math.normalizesafe(projectileTransform.Forward())));
            }

            state.EntityManager.SetComponentEnabled<ShouldSpawnProjectile>(spawnerEntity, false);
        }
    }
}

[UpdateBefore(typeof(SpawnProjectilesSystem))]
public partial struct PlaySoundOnProjectileFireSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //state.RequireForUpdate<DynamicBuffer<AudioBufferData>>();
        state.RequireForUpdate<PlaySoundOnProjectileFireComponent>();
        state.RequireForUpdate<RandomComponent>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<AudioBufferData> audioBuffer)) return;
        //
        // var random = SystemAPI.GetSingletonRW<RandomComponent>();
        //
        // foreach (var playSound in SystemAPI.Query<PlaySoundOnProjectileFireComponent>()
        //         .WithAll<ShouldSpawnProjectile>())
        // {
        //     float randomFloat = random.ValueRW.random.NextFloat();
        //
        //     if (randomFloat > playSound.ChanceToPlay)
        //     {
        //         Debug.Log("Play spawn sound!");
        //         audioBuffer.Add(new AudioBufferData {AudioData = playSound.AudioData});
        //     }
        // }
    }
}


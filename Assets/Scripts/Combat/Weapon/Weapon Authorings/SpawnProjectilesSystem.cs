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
        state.RequireForUpdate<HammerComponent>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // Spawn projectile
        foreach (var (  spawnerTransform, projectileSpawner, entity) 
            in SystemAPI.Query< LocalTransform, ProjectileSpawnerComponent>()
                .WithAll<ShouldSpawnProjectile>()
                .WithEntityAccess())
        {
            Entity projectileEntity = entityManager.Instantiate(projectileSpawner.Projectile);
            var projectileTransform = entityManager.GetComponentData<LocalTransform>(projectileEntity);

            bool hasWeapon = entityManager.HasComponent<WeaponComponent>(entity);
            if (hasWeapon)
            {
                var weapon = entityManager.GetComponentData<WeaponComponent>(entity);
                
                projectileTransform.Position = weapon.AttackPoint.Position;
                projectileTransform.Rotation = math.mul(weapon.AttackPoint.Rotation, projectileTransform.Rotation);
                
                if (entityManager.HasComponent<OwnerWeapon>(projectileEntity))
                {
                    // set owner data
                    entityManager.SetComponentData(projectileEntity, new OwnerWeapon
                    {
                        Value = entity,
                        OwnerWasActive = weapon.InActiveState
                    });
                }
            }
            else
            {
                projectileTransform.Position = spawnerTransform.Position;
                projectileTransform.Rotation = math.mul(spawnerTransform.Rotation, projectileTransform.Rotation);
            }

            // set new transform values and direction
            entityManager.SetComponentData(projectileEntity, projectileTransform);
            entityManager.SetComponentData(projectileEntity, new DirectionComponent(math.normalizesafe(projectileTransform.Forward())));

            

            state.EntityManager.SetComponentEnabled<ShouldSpawnProjectile>(entity, false);
        }
    }
}
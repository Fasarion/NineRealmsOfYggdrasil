using Movement;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Weapon;


[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct HammerAttackSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HammerComponent>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        var entityManager = state.EntityManager;

        // Spawn projectile
        foreach (var (  weapon, hammer, projectileSpawner, entity) 
            in SystemAPI.Query< WeaponComponent, HammerComponent, ProjectileSpawnerComponent>()
                .WithAll<DoNextFrame>()
                .WithEntityAccess())
        {
            Entity projectileEntity = entityManager.Instantiate(projectileSpawner.Projectile);
            var entityTransform = entityManager.GetComponentData<LocalTransform>(projectileEntity);
            
            entityTransform.Position = weapon.AttackPoint.Position;
            entityTransform.Rotation = math.mul(weapon.AttackPoint.Rotation, entityTransform.Rotation);
        
            // set new transform values and direction
            entityManager.SetComponentData(projectileEntity, entityTransform);
            entityManager.SetComponentData(projectileEntity, new DirectionComponent(math.normalizesafe(entityTransform.Forward())));

            // set owner data
            // TODO: remove?
            entityManager.SetComponentData(projectileEntity, new OwnerWeapon{Value = entity});

            // // Add projectile to projectile buffer
            // if (entityManager.HasBuffer<ProjectileBufferElement>(entity))
            // {
            //     var projectileBuffer = entityManager.GetBuffer<ProjectileBufferElement>(entity);
            //     projectileBuffer.Add(new ProjectileBufferElement {Projectile = projectileEntity});
            // }
        } 
        
        // remove DoNextFrame tag to avoid repeating system in the next frame
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (  weapon, hammer, projectileSpawner, entity) 
            in SystemAPI.Query< WeaponComponent, HammerComponent, ProjectileSpawnerComponent>()
                .WithAll<DoNextFrame>()
                .WithEntityAccess())
        {
            ecb.RemoveComponent<DoNextFrame>(entity);
        } 
        
        ecb.Playback(state.EntityManager);
    }
}
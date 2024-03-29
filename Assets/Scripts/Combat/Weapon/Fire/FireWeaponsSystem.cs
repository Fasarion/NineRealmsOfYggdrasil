using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Movement;
using Player;

namespace Weapon
{
    
    [UpdateAfter(typeof(PlayerWantsToFireSystem))]

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct FireWeaponsSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AimSettingsData>();
        }
    
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
        
            foreach (var (weapon, transform, ltw) in 
                SystemAPI.Query<RefRO<WeaponComponent>, RefRO<LocalTransform>, RefRO<LocalToWorld>>())
            {
                if (!weapon.ValueRO.WantsToFire)
                    return;

                SpawnProjectile(ref state, entityManager, weapon, transform, ltw);
            }
        
        }

        [BurstCompile]
        private void SpawnProjectile(ref SystemState state, EntityManager entityManager, 
            RefRO<WeaponComponent> weapon, RefRO<LocalTransform> transform, RefRO<LocalToWorld> ltw)
        {
            Entity projectileEntity = entityManager.Instantiate(weapon.ValueRO.Projectile);
            var entityTransform = entityManager.GetComponentData<LocalTransform>(projectileEntity);
        
            // projectile position
            entityTransform.Position = transform.ValueRO.Position;
        
            // projectile rotation
            entityTransform.Rotation = math.mul(ltw.ValueRO.Rotation, entityTransform.Rotation);
        
            // set new transform values and direction
            entityManager.SetComponentData(projectileEntity, entityTransform);
            entityManager.SetComponentData(projectileEntity, new DirectionComponent(math.normalizesafe(transform.ValueRO.Forward())));
        }
    }
}

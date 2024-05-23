using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace Weapon
{
    public class ProjectileSpawnerAuthoring : MonoBehaviour 
    {
        [Tooltip("Prefab of the projectile that this weapon launches.")]
        [SerializeField] private GameObject projectilePrefab;

        class Baker : Baker<ProjectileSpawnerAuthoring>
        {
            public override void Bake(ProjectileSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new ProjectileSpawnerComponent
                {
                    Projectile = GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic),
                });
                
                AddComponent(entity, new ShouldSpawnProjectile());
                SetComponentEnabled<ShouldSpawnProjectile>(entity, false);
            }
        }
    }
}


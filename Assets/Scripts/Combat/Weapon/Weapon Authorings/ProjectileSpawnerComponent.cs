using Unity.Entities;

namespace Weapon
{
    public struct ProjectileSpawnerComponent : IComponentData
    {
        public Entity Projectile;
    }
    
    public struct ShouldSpawnProjectile : IComponentData, IEnableableComponent{}
}
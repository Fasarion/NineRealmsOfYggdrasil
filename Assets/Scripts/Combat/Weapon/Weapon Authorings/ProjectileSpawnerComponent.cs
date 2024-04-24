using Unity.Entities;

namespace Weapon
{
    public struct ProjectileSpawnerComponent : IComponentData
    {
        public Entity Projectile;
        // public float CoolDownTime;
        // public float CurrentCoolDownTime;
        // public bool WantsToFire;
        //
        // public bool HasCooledDown => CurrentCoolDownTime > CoolDownTime;
    }
    
    public struct ShouldSpawnProjectile : IComponentData, IEnableableComponent{}
}
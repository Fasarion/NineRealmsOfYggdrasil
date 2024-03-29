using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace Weapon
{
    public class WeaponAuthoring : MonoBehaviour 
    {
        [Tooltip("Prefab of the projectile that this weapon launches.")]
        [SerializeField] private GameObject projectilePrefab;
        
        [Tooltip("Cooldown time between each shot coming from this weapon.")]
        [SerializeField] private float coolDownTime;
    
        class Baker : Baker<WeaponAuthoring>
        {
            public override void Bake(WeaponAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new WeaponComponent
                {
                    Projectile = GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic),
                    CoolDownTime = authoring.coolDownTime,
                
                    CurrentCoolDownTime = authoring.coolDownTime
                });
            }
        }
    }

    public struct WeaponComponent : IComponentData
    {
        public Entity Projectile;
        public float CoolDownTime;
        public float CurrentCoolDownTime;
        public bool WantsToFire;

        public bool HasCooledDown => CurrentCoolDownTime > CoolDownTime;
    }
}


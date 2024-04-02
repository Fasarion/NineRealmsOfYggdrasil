using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace Weapon
{
    public class GunAuthoring : MonoBehaviour 
    {
        [Tooltip("Prefab of the projectile that this weapon launches.")]
        [SerializeField] private GameObject projectilePrefab;
        
        [Tooltip("Cooldown time between each shot coming from this weapon.")]
        [SerializeField] private float coolDownTime;
    
        class Baker : Baker<GunAuthoring>
        {
            public override void Bake(GunAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new GunComponent
                {
                    Projectile = GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic),
                    CoolDownTime = authoring.coolDownTime,
                
                    CurrentCoolDownTime = authoring.coolDownTime
                });
            }
        }
    }

    public struct GunComponent : IComponentData
    {
        public Entity Projectile;
        public float CoolDownTime;
        public float CurrentCoolDownTime;
        public bool WantsToFire;

        public bool HasCooledDown => CurrentCoolDownTime > CoolDownTime;
    }
}


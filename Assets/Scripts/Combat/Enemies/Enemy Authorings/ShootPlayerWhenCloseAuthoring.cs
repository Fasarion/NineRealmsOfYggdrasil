using Unity.Entities;
using UnityEngine;

public class ShootPlayerWhenCloseAuthoring : MonoBehaviour
{
    [SerializeField] private float shootingCooldown;
    [SerializeField] private float shootDistance;
    
    class Baker : Baker<ShootPlayerWhenCloseAuthoring>
    {
        public override void Bake(ShootPlayerWhenCloseAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootPlayerWhenCloseComponent
            {
                ShootingCooldownTime = authoring.shootingCooldown,
                MinimumDistanceForShootingSquared = authoring.shootDistance * authoring.shootDistance
            });
        }
    }
}
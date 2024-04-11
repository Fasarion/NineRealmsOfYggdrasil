using Unity.Entities;
using UnityEngine;

namespace Weapon
{
    public class ProjectileBufferAuthoring : MonoBehaviour
    {
        class Baker : Baker<ProjectileBufferAuthoring>
        {
            public override void Bake(ProjectileBufferAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddBuffer<ProjectileBufferElement>(entity);
            }
        }
    }

    public struct ProjectileBufferElement : IBufferElementData
    {
        public Entity Projectile;
    }
}
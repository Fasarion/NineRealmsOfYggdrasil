using Unity.Entities;
using Unity.Mathematics;

namespace Health
{
    public struct HealthBarOffset : IComponentData
    {
        // Managed Cleanup Component used to reference the world-space Unity UI health bar slider associated with an entity
        public float3 Value;
    }
}
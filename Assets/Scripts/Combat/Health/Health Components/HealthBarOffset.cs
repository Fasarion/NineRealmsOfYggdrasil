using Unity.Entities;
using Unity.Mathematics;

namespace Health
{
    public struct HealthBarOffset : IComponentData
    {
        public float3 Value;
    }
}
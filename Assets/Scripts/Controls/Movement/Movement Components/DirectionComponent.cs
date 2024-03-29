using Unity.Entities;
using Unity.Mathematics;

namespace Movement
{
    public struct DirectionComponent : IComponentData, IEnableableComponent
    {
        public DirectionComponent(float3 value)
        {
            Value = value;
        }
    
        public float3 Value;
    }
}
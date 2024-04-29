using Unity.Entities;
using Unity.Mathematics;

namespace Player
{
    public struct PlayerPositionSingleton : IComponentData
    {
        public float3 Value;
    }
    
    public struct PlayerRotationSingleton : IComponentData
    {
        public quaternion Value;
    }
}
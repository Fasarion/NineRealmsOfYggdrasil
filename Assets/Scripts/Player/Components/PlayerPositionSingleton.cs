using Unity.Entities;
using Unity.Mathematics;

namespace Player
{
    public struct PlayerPositionSingleton : IComponentData
    {
        public float3 Value;
    }
}
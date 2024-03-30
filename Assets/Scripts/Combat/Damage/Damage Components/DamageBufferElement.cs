using Unity.Entities;

namespace Damage
{
    public struct DamageBufferElement : IBufferElementData
    {
        public DamageType DamageType;
        public float HitPoints;
    }
}
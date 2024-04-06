using Unity.Entities;

namespace Damage
{
    public struct KnockBackForce : IComponentData
    {
        public float Value;
        public bool KnockAwayFromPlayer;
    }
}
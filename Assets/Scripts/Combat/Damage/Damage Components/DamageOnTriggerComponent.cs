using Unity.Entities;

namespace Damage
{
    /// <summary>
    /// Entities with this components will deal damage upon a trigger with another entity that has HP.
    /// </summary>
    public struct DamageOnTriggerComponent : IComponentData
    {
        public float DamageValue;
        public DamageType DamageType;
    }

    public struct KnockBackForce : IComponentData
    {
        public float Value;
        public bool KnockAwayFromPlayer;
    }
}
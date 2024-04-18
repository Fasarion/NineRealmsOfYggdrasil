using Unity.Entities;
using UnityEngine;

namespace Damage
{
    /// <summary>
    /// Entities with this components will deal damage upon a trigger with another entity that has HP.
    /// </summary>
    public struct DamageOnTriggerComponent : IComponentData
    {
        public DamageContents Value;
    }

    [System.Serializable]
    public struct DamageContents
    {
        [Header("Damage")]
        [Tooltip("How much damage will this object inflict upon hit?")]
        public float DamageValue;
        
        [Tooltip("What type of damage will this object inflict?")]
        public DamageType DamageType;

        [Header("Critical")] 
        [Range(0,1)]
        public float CriticalRate;
        public float CriticalModifier;
    }
}
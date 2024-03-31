using System.Collections;
using System.Collections.Generic;
using Damage;
using Unity.Entities;
using UnityEngine;

namespace Damage
{
    public class DamageOnTriggerAuthoring : MonoBehaviour
    {
        [Tooltip("How much damage will this object inflict upon trigger?")]
        [SerializeField] private float damageValue = 1f;

        [Tooltip("What type of damage will this object inflict?")]
        [SerializeField] private DamageType damageType;
    
        class Baker : Baker<DamageOnTriggerAuthoring>
        {
            public override void Bake(DamageOnTriggerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
            
                AddComponent(entity, new DamageOnTriggerComponent
                {
                    DamageValue = authoring.damageValue,
                    DamageType = authoring.damageType
                });
            }
        }
    }
}


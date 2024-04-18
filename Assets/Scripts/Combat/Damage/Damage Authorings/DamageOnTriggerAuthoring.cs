using System.Collections;
using System.Collections.Generic;
using Damage;
using Unity.Entities;
using UnityEngine;

namespace Damage
{
    public class DamageOnTriggerAuthoring : MonoBehaviour
    {
        [SerializeField] private DamageContents damageContents;
    
        class Baker : Baker<DamageOnTriggerAuthoring>
        {
            public override void Bake(DamageOnTriggerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
            
                AddComponent(entity, new DamageOnTriggerComponent
                {
                    Value = authoring.damageContents
                });
            }
        }
    }
}


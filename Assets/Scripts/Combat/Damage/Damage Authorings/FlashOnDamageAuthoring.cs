using System;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Damage
{
    public class FlashOnDamageAuthoring : MonoBehaviour
    {
        [Tooltip("Duration of flash for the entity when taken damage.")]
        [SerializeField] private float flashTime;
        
        class Baker : Baker<FlashOnDamageAuthoring>
        {
            public override void Bake(FlashOnDamageAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
            
                // Flash Timer
                AddComponent(entity, new DamageFlashTimer()
                {
                   FlashTime = authoring.flashTime
                });
                SetComponentEnabled<DamageFlashTimer>(entity, false);
            }
        }
    }
}
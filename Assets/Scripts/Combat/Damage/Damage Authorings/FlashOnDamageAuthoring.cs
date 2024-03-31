using Unity.Entities;
using Unity.Mathematics;
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
            
                AddComponent(entity, new DamageFlashTimer()
                {
                   FlashTime = authoring.flashTime
                });
                SetComponentEnabled<DamageFlashTimer>(entity, false);
                
                AddComponent(entity, new URPMaterialPropertyBaseColor { Value = new float4(1, 1, 1, 1) });
            }
        }
    }
}
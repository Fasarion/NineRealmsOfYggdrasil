using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Health
{
    public class HealthBarAuthoring : MonoBehaviour
    {
        [Tooltip("The health bar's offset from the entity's position.")]
        [SerializeField] private Vector3 offset;
        
        private class HitPointsBaker : Baker<HealthBarAuthoring>
        {
            public override void Bake(HealthBarAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new HealthBarOffset { Value = authoring.offset });
                AddComponent<UpdateHealthBarUI>(entity);
                SetComponentEnabled<UpdateHealthBarUI>(entity, false);
                
                AddComponent(entity, new URPMaterialPropertyBaseColor { Value = new float4(1, 1, 1, 1) });
            }
        }
    }
}


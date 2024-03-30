using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace Health
{
    public class HealthBarAuthoring : MonoBehaviour
    {
        [Tooltip("Prefab of health bar that will spawn and hover above entity.")]
        [SerializeField] private GameObject healthBarPrefab;
        
        [Tooltip("The health bar's offset from the entity's position.")]
        [SerializeField] private Vector3 healthBarOffset;

        private class Baker : Baker<HealthBarAuthoring>
        {
            public override void Bake(HealthBarAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                // Set prefab
                AddComponentObject(entity, new HealthBarUIPrefabComponent{ Value = authoring.healthBarPrefab});

                // Set offset
                AddComponent(entity, new HealthBarOffset { Value = authoring.healthBarOffset });
                
                // Set update tag to false to begin with
                AddComponent<UpdateHealthBarUI>(entity);
                SetComponentEnabled<UpdateHealthBarUI>(entity, false);
                
                // // Color
                // AddComponent(entity, new URPMaterialPropertyBaseColor { Value = new float4(1, 1, 1, 1) });
            }
        }
    }
}


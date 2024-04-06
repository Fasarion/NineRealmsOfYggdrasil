using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Damage
{
    public class MeshColorAuthoring : MonoBehaviour
    {
        [Tooltip("Color of this entity's mesh.")]
        [SerializeField] private Color color;
        

        class Baker : Baker<MeshColorAuthoring>
        {
            public override void Bake(MeshColorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                // Setup Base Color
                var materialColor = authoring.color;
                float4 color = new float4(materialColor.r, materialColor.g, materialColor.b, materialColor.a);
                AddComponent(entity, new MeshColor()
                {
                    Value = color
                });
                
                AddComponent(entity, new URPMaterialPropertyBaseColor { Value = color});
            }
        }
    }
}
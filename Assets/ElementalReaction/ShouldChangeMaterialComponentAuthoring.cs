using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ShouldChangeMaterialComponentAuthoring : MonoBehaviour
{
    public MaterialType materialType;

    public class ShouldChangeMaterialComponentAuthoringBaker : Baker<ShouldChangeMaterialComponentAuthoring>
    {
        public override void Bake(ShouldChangeMaterialComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ShouldChangeMaterialComponent { MaterialType = authoring.materialType });
        }
    }
}

public struct ShouldChangeMaterialComponent : IComponentData
{
    public MaterialType MaterialType;
}

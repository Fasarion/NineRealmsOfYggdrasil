using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BaseMaterialReferenceComponentAuthoring : MonoBehaviour
{
    public MaterialType type;

    public class BaseMaterialReferenceComponentAuthoringBaker : Baker<BaseMaterialReferenceComponentAuthoring>
    {
        public override void Bake(BaseMaterialReferenceComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BaseMaterialReferenceComponent { Type = authoring.type });
        }
    }
}


public struct BaseMaterialReferenceComponent : IComponentData
{
    public MaterialType Type;
}

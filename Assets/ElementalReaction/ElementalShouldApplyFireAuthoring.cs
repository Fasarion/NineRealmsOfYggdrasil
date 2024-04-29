using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalShouldApplyFireAuthoring : MonoBehaviour
{
    public int stacksPerHit;

    public class ElementalShouldApplyFireAuthoringBaker : Baker<ElementalShouldApplyFireAuthoring>
    {
        public override void Bake(ElementalShouldApplyFireAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ElementalShouldApplyFireComponent { StacksPerHit = authoring.stacksPerHit });
        }
    }
}

public struct ElementalShouldApplyFireComponent : IComponentData
{
    public int StacksPerHit;
}

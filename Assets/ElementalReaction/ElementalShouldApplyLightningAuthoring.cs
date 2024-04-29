using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalShouldApplyLightningAuthoring : MonoBehaviour
{
    public int stacksPerHit;

    public class ElementalShouldApplyLightningAuthoringBaker : Baker<ElementalShouldApplyLightningAuthoring>
    {
        public override void Bake(ElementalShouldApplyLightningAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalShouldApplyLightningComponent { StacksPerHit = authoring.stacksPerHit });
        }
    }
}

public struct ElementalShouldApplyLightningComponent : IComponentData
{
    public int StacksPerHit;
}

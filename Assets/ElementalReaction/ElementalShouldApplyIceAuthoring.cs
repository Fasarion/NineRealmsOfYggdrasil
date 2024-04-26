using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalShouldApplyIceAuthoring : MonoBehaviour
{
    public int stacksPerHit;

    public class ElementalShouldApplyIceAuthoringBaker : Baker<ElementalShouldApplyIceAuthoring>
    {
        public override void Bake(ElementalShouldApplyIceAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ElementalShouldApplyIceComponent { StacksPerHit = authoring.stacksPerHit });
        }
    }
}

public struct ElementalShouldApplyIceComponent : IComponentData
{
    public int StacksPerHit;
}

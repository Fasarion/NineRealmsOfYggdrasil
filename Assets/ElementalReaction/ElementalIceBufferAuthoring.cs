using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalIceBufferAuthoring : MonoBehaviour
{
    public int stacksPerHit;

    public class ElementalIceBufferAuthoringBaker : Baker<ElementalIceBufferAuthoring>
    {
        public override void Bake(ElementalIceBufferAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ElementalIceBuffer { StacksPerHit = authoring.stacksPerHit });

            AddBuffer<ElementalIceBufferElement>(entity);
        }
    }
}

public struct ElementalIceBuffer : IComponentData
{
    public int StacksPerHit;
}

public struct ElementalIceBufferElement : IBufferElementData
{
    public int Stacks;
}

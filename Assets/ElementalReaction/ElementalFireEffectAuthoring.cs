using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalFireEffectAuthoring : MonoBehaviour
{
    public float currentDurationTime;

    public class ElementalFireEffectAuthoringBaker : Baker<ElementalFireEffectAuthoring>
    {
        public override void Bake(ElementalFireEffectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalFireEffectComponent { CurrentDurationTime = authoring.currentDurationTime });
        }
    }
}

public struct ElementalFireEffectComponent : IComponentData
{
    public float CurrentDurationTime;
}


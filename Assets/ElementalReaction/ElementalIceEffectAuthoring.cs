using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalIceEffectAuthoring : MonoBehaviour
{
    public float currentDurationTime;

    public class ElementalIceEffectAuthoringBaker : Baker<ElementalIceEffectAuthoring>
    {
        public override void Bake(ElementalIceEffectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalIceEffectComponent { CurrentDurationTime = authoring.currentDurationTime });
        }
    }
}

public struct ElementalIceEffectComponent : IComponentData
{
    public float CurrentDurationTime;
}

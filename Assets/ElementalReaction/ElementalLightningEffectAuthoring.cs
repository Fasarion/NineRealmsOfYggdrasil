using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalLightningEffectAuthoring : MonoBehaviour
{
    public float currentDurationTime;

    public class ElementalLightningEffectAuthoringBaker : Baker<ElementalLightningEffectAuthoring>
    {
        public override void Bake(ElementalLightningEffectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalLightningEffectComponent { CurrentDurationTime = authoring.currentDurationTime });
        }
    }
}

public struct ElementalLightningEffectComponent : IComponentData
{
    public float CurrentDurationTime;
}


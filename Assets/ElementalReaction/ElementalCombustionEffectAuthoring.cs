using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalCombustionEffectAuthoring : MonoBehaviour
{
    public bool hasBeenApplied;

    public class ElementalCombustionEffectAuthoringBaker : Baker<ElementalCombustionEffectAuthoring>
    {
        public override void Bake(ElementalCombustionEffectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalCombustionEffectComponent
                {
                    HasBeenApplied = authoring.hasBeenApplied,
                });
        }
    }
}

public struct ElementalCombustionEffectComponent : IComponentData
{

    public bool HasBeenApplied;
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalConductEffectConfigAuthoring : MonoBehaviour
{
    [Header("Conduct")]
    [Tooltip(
        "The additional jumps that will be added to electrical effects like chain lightning. 1 means that the target will make the lightning jump to 1 additional target, 2 means 2 additional targets and so on.")]
    public int conductSpreadAddition;

    public float conductDuration;
    public float maxConductStacks;

    public class ElementalConductEffectConfigAuthoringBaker : Baker<ElementalConductEffectConfigAuthoring>
    {
        public override void Bake(ElementalConductEffectConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalConductEffectConfig
                {
                    ConductSpreadAddition = authoring.conductSpreadAddition,
                    ConductDuration = authoring.conductDuration,
                    MaxConductStacks = authoring.maxConductStacks
                });
        }
    }
}

public struct ElementalConductEffectConfig : IComponentData
{
    public int ConductSpreadAddition;
    public float ConductDuration;
    public float MaxConductStacks;
}

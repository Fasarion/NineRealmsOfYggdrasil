using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalVulnerableEffectAuthoring : MonoBehaviour
{
    public float currentDurationTime;
    public bool hasBeenApplied;
    public int stacks;

    public class ElementalVulnerableEffectAuthoringBaker : Baker<ElementalVulnerableEffectAuthoring>
    {
        public override void Bake(ElementalVulnerableEffectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalVulnerableEffectComponent
                {
                    CurrentDurationTime = authoring.currentDurationTime,
                    HasBeenApplied = authoring.hasBeenApplied,
                    Stacks = authoring.stacks
                });
        }
    }
}

public struct ElementalVulnerableEffectComponent : IComponentData
{
    public float CurrentDurationTime;
    public bool HasBeenApplied;
    public int Stacks;
}

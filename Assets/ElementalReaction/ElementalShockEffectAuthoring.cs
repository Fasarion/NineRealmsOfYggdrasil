using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalShockEffectAuthoring : MonoBehaviour
{
    public float currentDurationTime;
    public bool hasBeenApplied;
    public int stacks;

    public class ElementalShockEffectAuthoringBaker : Baker<ElementalShockEffectAuthoring>
    {
        public override void Bake(ElementalShockEffectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalShockEffectComponent
                {
                    CurrentDurationTime = authoring.currentDurationTime,
                    HasBeenApplied = authoring.hasBeenApplied,
                    Stacks = authoring.stacks
                });
        }
    }
}

public struct ElementalShockEffectComponent : IComponentData
{
    public float CurrentDurationTime;
    public bool HasBeenApplied;
    public int Stacks;
}

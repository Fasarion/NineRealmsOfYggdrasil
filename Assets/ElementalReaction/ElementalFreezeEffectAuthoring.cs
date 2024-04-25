using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalFreezeEffectAuthoring : MonoBehaviour
{
    public float currentDurationTime;
    public bool hasBeenApplied;
    public int stacks;

    public class ElementalFreezeEffectAuthoringBaker : Baker<ElementalFreezeEffectAuthoring>
    {
        public override void Bake(ElementalFreezeEffectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalFreezeEffectComponent
                {
                    CurrentDurationTime = authoring.currentDurationTime,
                    HasBeenApplied = authoring.hasBeenApplied,
                    Stacks = authoring.stacks
                });
        }
    }
}

public struct ElementalFreezeEffectComponent : IComponentData
{
    public float CurrentDurationTime;
    public bool HasBeenApplied;
    public int Stacks;
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalBurnEffectAuthoring : MonoBehaviour
{
    public float currentDurationTime;
    public bool hasBeenApplied;
    public int stacks;

    public class ElementalBurnEffectAuthoringBaker : Baker<ElementalBurnEffectAuthoring>
    {
        public override void Bake(ElementalBurnEffectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalBurnEffectComponent
                {
                    CurrentDurationTime = authoring.currentDurationTime,
                    HasBeenApplied = authoring.hasBeenApplied,
                    Stacks = authoring.stacks
                });
        }
    }
}

public struct ElementalBurnEffectComponent : IComponentData
{
    public float CurrentDurationTime;
    public bool HasBeenApplied;
    public int Stacks;
}

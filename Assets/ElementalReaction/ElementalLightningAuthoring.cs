using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalLightningAuthoring : MonoBehaviour
{
    public float currentDurationTime;
    public bool hasBeenApplied;
    public int stacks;

    public class ElementalLightningAuthoringBaker : Baker<ElementalLightningAuthoring>
    {
        public override void Bake(ElementalLightningAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalLightningComponent
                {
                    CurrentDurationTime = authoring.currentDurationTime,
                    HasBeenApplied = authoring.hasBeenApplied,
                    Stacks = authoring.stacks
                });
        }
    }
}

public struct ElementalLightningComponent : IComponentData
{
    public float CurrentDurationTime;
    public bool HasBeenApplied;
    public int Stacks;
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalFreezeEffectConfigAuthoring : MonoBehaviour
{
    [Header("Freeze")]
    [Tooltip(
        "How much freeze slows movement speed per stack on a scale from 0 to 1. 0.5 for example means that the movement is slowed by 50% while 1 means the movement is completely stopped.")]
    public float freezeMovementSlowPercentage;

    public float freezeDuration;
    public float maxFreezeStacks;

    public class ElementalFreezeEffectConfigAuthoringBaker : Baker<ElementalFreezeEffectConfigAuthoring>
    {
        public override void Bake(ElementalFreezeEffectConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalFreezeEffectConfig
                {
                    FreezeMovementSlowPercentage = authoring.freezeMovementSlowPercentage,
                    FreezeDuration = authoring.freezeDuration,
                    MaxFreezeStacks = authoring.maxFreezeStacks
                });
        }
    }
}

public struct ElementalFreezeEffectConfig : IComponentData
{
    public float FreezeMovementSlowPercentage;
    public float FreezeDuration;
    public float MaxFreezeStacks;
}

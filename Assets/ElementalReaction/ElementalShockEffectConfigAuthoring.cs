using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalShockEffectConfigAuthoring : MonoBehaviour
{
    [Header("Shock")] [HideInInspector] public float shockDamage;
    public float shockDelay;
    public float maxShockStacks;

    public class ElementalShockEffectConfigAuthoringBaker : Baker<ElementalShockEffectConfigAuthoring>
    {
        public override void Bake(ElementalShockEffectConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalShockEffectConfig
                {
                    ShockDamage = authoring.shockDamage,
                    ShockDelay = authoring.shockDelay,
                    MaxShockStacks = authoring.maxShockStacks
                });
        }
    }
}

public struct ElementalShockEffectConfig : IComponentData
{
    public float ShockDamage;
    public float ShockDelay;
    public float MaxShockStacks;
}

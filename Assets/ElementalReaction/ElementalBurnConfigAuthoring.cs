using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalBurnConfigAuthoring : MonoBehaviour
{
    [Header("Burn")] 
    [HideInInspector] public float burnDamage;
    public float burnDuration;
    public float maxBurnStacks;
    [Tooltip("How much time in seconds passes between burn damage ticks.")]
    public float burnDamageTicksTime;

    public class ElementalBurnConfigAuthoringBaker : Baker<ElementalBurnConfigAuthoring>
    {
        public override void Bake(ElementalBurnConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalBurnConfig
                {
                    BurnDamage = authoring.burnDamage,
                    BurnDuration = authoring.burnDuration,
                    MaxBurnStacks = authoring.maxBurnStacks,
                    BurnDamageTicksTime = authoring.burnDamageTicksTime
                });
        }
    }
}

public struct ElementalBurnConfig : IComponentData
{
    public float BurnDamage;
    public float BurnDuration;
    public float MaxBurnStacks;
    public float BurnDamageTicksTime;
}

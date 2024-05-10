using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HammerPassiveAbilityConfigAuthoring : MonoBehaviour
{
    public float initialRadius;
    public float chainRadius;
    public int strikeCount;
    public float timeBetweenStrikes;
    public GameObject abilityPrefab;
    public float vfxHeightOffset;

    public class HammerPassiveAbilityConfigAuthoringBaker : Baker<HammerPassiveAbilityConfigAuthoring>
    {
        public override void Bake(HammerPassiveAbilityConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new HammerPassiveAbilityConfig
                {
                    InitialRadius = authoring.initialRadius,
                    ChainRadius = authoring.chainRadius,
                    StrikeCount = authoring.strikeCount,
                    TimeBetweenStrikes = authoring.timeBetweenStrikes,
                    AbilityPrefab = GetEntity(authoring.abilityPrefab, TransformUsageFlags.Dynamic),
                    VFXHeightOffset = authoring.vfxHeightOffset
                });
        }
    }
}

public struct HammerPassiveAbilityConfig : IComponentData
{
    public float InitialRadius;
    public float ChainRadius;
    public int StrikeCount;
    public float TimeBetweenStrikes;
    public Entity AbilityPrefab;
    public float VFXHeightOffset;
}

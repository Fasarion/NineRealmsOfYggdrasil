using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HammerPassiveAbilityConfigAuthoring : MonoBehaviour
{
    [Tooltip("The distance from the player that the first lightning bolt is allowed to strike")]
    public float initialRadius;
    [Tooltip("The distance from the first enemy struck that subsequent lightning bolts are allowed to strike")]
    public float chainRadius;
    [Tooltip("The number of lightning strikes per passive trigger")]
    public int strikeCount;
    [Tooltip("The time between lightning bolt spawns in seconds")]
    public float timeBetweenStrikes;
    public GameObject abilityPrefab;
    public float vfxHeightOffset;
    
    [Header("Audio")] 
    [Tooltip("Sound to play when lightning strikes.")]
    [SerializeField] private AudioData hitAudio;

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
                    VFXHeightOffset = authoring.vfxHeightOffset,
                    HitAudio = authoring.hitAudio
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

    public AudioData HitAudio;
}

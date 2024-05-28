using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class ThunderBoltConfigAuthoring : MonoBehaviour
{
    public GameObject abilityPrefab;
    public GameObject projectilePrefab;
    public float maxDisplayTime;
    public float damageDelayTime;
    public float maxArea; 
    public int maxStrikes;
    public float strikeSpacing;
    public float timeBetweenStrikes;
    public float vfxHeightOffset;
    
    [Header("Audio")] 
    [Tooltip("Sound to play when lightning strikes.")]
    [SerializeField] private AudioData hitAudio;

    public class ThunderBoltConfigAuthoringBaker : Baker<ThunderBoltConfigAuthoring>
    {
        public override void Bake(ThunderBoltConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ThunderBoltConfig
                {
                    AbilityPrefab = GetEntity(authoring.abilityPrefab, TransformUsageFlags.Dynamic),
                    ProjectilePrefab = GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic),
                    MaxDisplayTime = authoring.maxDisplayTime,
                    DamageDelayTime = authoring.damageDelayTime,
                    MaxArea = authoring.maxArea,
                    MaxStrikes = authoring.maxStrikes,
                    StrikeSpacing = authoring.strikeSpacing,
                    TimeBetweenStrikes = authoring.timeBetweenStrikes,
                    VfxHeightOffset = authoring.vfxHeightOffset,
                    
                    HitAudio = authoring.hitAudio
                });

        }
    }
}

public struct ThunderBoltConfig : IComponentData
{
    public Entity AbilityPrefab;
    public Entity ProjectilePrefab;
    public float MaxDisplayTime;
    public float DamageDelayTime;
    public float MaxArea;
    public int MaxStrikes;
    public float StrikeSpacing;
    public float TimeBetweenStrikes;
    public float VfxHeightOffset;

    public AudioData HitAudio;
}

public struct TargetBufferElement : IBufferElementData
{
    public float3 Position;
}

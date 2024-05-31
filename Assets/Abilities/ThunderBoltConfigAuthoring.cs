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
    public GameObject sparkEffectPrefab;
    public float maxDisplayTime;
    public float damageDelayTime;
    public float maxArea; 
    public int maxStrikes;
    public float strikeSpacing;
    public float timeBetweenStrikes;
    public float vfxHeightOffset;
    public int maxRows;
    public float rowsAngle;
    
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
                    SparkEffectPrefab = GetEntity(authoring.sparkEffectPrefab, TransformUsageFlags.Dynamic),
                    MaxDisplayTime = authoring.maxDisplayTime,
                    DamageDelayTime = authoring.damageDelayTime,
                    MaxArea = authoring.maxArea,
                    MaxStrikes = authoring.maxStrikes,
                    StrikeSpacing = authoring.strikeSpacing,
                    TimeBetweenStrikes = authoring.timeBetweenStrikes,
                    VfxHeightOffset = authoring.vfxHeightOffset,
                    MaxRows = authoring.maxRows,
                    RowsAngle = authoring.rowsAngle,
                    
                    HitAudio = authoring.hitAudio
                });

        }
    }
}

public struct ThunderBoltConfig : IComponentData
{
    public Entity AbilityPrefab;
    public Entity ProjectilePrefab;
    public Entity SparkEffectPrefab;
    public float MaxDisplayTime;
    public float DamageDelayTime;
    public float MaxArea;
    public int MaxStrikes;
    public float StrikeSpacing;
    public float TimeBetweenStrikes;
    public float VfxHeightOffset;
    public int MaxRows;
    public float RowsAngle;

    public AudioData HitAudio;
}

public struct TargetBufferElement : IBufferElementData
{
    public float3 Position;
}

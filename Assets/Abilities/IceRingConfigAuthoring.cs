using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class IceRingConfigAuthoring : MonoBehaviour
{
    public GameObject abilityPrefab;
    public GameObject chargeAreaPrefab;
    public float maxDisplayTime;
    public float damageDelayTime;
    [HideInInspector] public float maxChargeTime;
    [HideInInspector] public float maxArea;
    [HideInInspector] public float chargeSpeed;
   // public List<IceRingStage> abilityStages;
    public float vfxScale = 0.5f;
    public float chargeAreaVfxHeightOffset;
    public float abilityVfxHeightOffset;

    [HideInInspector] public float ogCachedDamageValue;
    [HideInInspector] public bool isInitialized;
    [HideInInspector] public bool isAbilityReleased;

    [Header("Audio")] 
    [SerializeField] private AudioData chargeAudioData;
    [SerializeField] private AudioData impactAudioData;
    

    public class IceRingConfigAuthoringBaker : Baker<IceRingConfigAuthoring>
    {
        public override void Bake(IceRingConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new IceRingConfig
                {
                    abilityPrefab = GetEntity(authoring.abilityPrefab, TransformUsageFlags.Dynamic),
                    chargeAreaPrefab = GetEntity(authoring.chargeAreaPrefab, TransformUsageFlags.Dynamic),
                    maxDisplayTime = authoring.maxDisplayTime,
                    damageDelayTime = authoring.damageDelayTime,
                    maxChargeTime = authoring.maxChargeTime,
                    maxArea = authoring.maxArea,
                    isAbilityReleased = authoring.isAbilityReleased,
                    chargeSpeed = authoring.chargeSpeed,
                    isInitialized = authoring.isInitialized,
                    vfxScale = authoring.vfxScale,
                    chargeAreaVfxHeightOffset = authoring.chargeAreaVfxHeightOffset,
                    abilityVfxHeightOffset = authoring.abilityVfxHeightOffset,
                  //  ogCachedDamageValue = authoring.ogCachedDamageValue,
                  
                  
                  chargeAudioData = authoring.chargeAudioData,
                  impactAudioData = authoring.impactAudioData
                });
            
            // var buffer = AddBuffer<IceRingStageElement>(entity);
            //
            // foreach (var stage in authoring.abilityStages)
            // {
            //     buffer.Add(new IceRingStageElement
            //     {
            //         damageModifier = stage.damageModifier,
            //         maxArea = stage.maxArea,
            //         maxChargeTime = stage.maxChargeTime,
            //     });
            // }
        }
    }
}

public struct IceRingConfig : IComponentData
{
    public Entity abilityPrefab;
    public Entity chargeAreaPrefab;
    public float maxDisplayTime;
    public float damageDelayTime;
    public float maxChargeTime;
    public float maxArea;
    public bool isAbilityReleased;
    public float chargeSpeed;
    public bool isInitialized;
    public float vfxScale;
    public float chargeAreaVfxHeightOffset;
    public float abilityVfxHeightOffset;
   // public float ogCachedDamageValue;
   
   public AudioData chargeAudioData;
   public AudioData impactAudioData;
}

// public struct IceRingStageElement : IBufferElementData
// {
//     public float damageModifier;
//     public float maxArea;
//     public float maxChargeTime;
// }
//
// [System.Serializable]
// public struct IceRingStage
// {
//     public float damageModifier;
//     public float maxArea;
//     public float maxChargeTime;
// }

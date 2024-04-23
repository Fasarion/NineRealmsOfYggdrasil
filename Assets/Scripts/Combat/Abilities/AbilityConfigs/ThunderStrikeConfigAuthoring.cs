using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class ThunderStrikeConfigAuthoring : MonoBehaviour
{
    [Header("Main Ability Values")]
    public GameObject mainAbilityPrefab;
    public int maxStrikes;
    public float timeBetweenStrikes;
    public float initialStrikeDelay;
    
    [Header("Shockwave Values")]
    public GameObject shockwaveAbilityPrefab;
    public float maxAftermathDisplayTime;
    public float damageDelayTime;
    public float damageArea;
    

    public class ThunderStrikeConfigAuthoringBaker : Baker<ThunderStrikeConfigAuthoring>
    {
        public override void Bake(ThunderStrikeConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ThunderStrikeConfig
                {
                    mainAbilityPrefab = GetEntity(authoring.mainAbilityPrefab, TransformUsageFlags.Dynamic),
                    maxAftermathDisplayTime = authoring.maxAftermathDisplayTime,
                    maxStrikes = authoring.maxStrikes,
                    timeBetweenStrikes = authoring.timeBetweenStrikes,
                    projectileAbilityPrefab = GetEntity(authoring.shockwaveAbilityPrefab, TransformUsageFlags.Dynamic),
                    damageArea = authoring.damageArea,
                    damageDelayTime = authoring.damageDelayTime,
                    initialStrikeDelay = authoring.initialStrikeDelay,
                });
        }
    }
}

public struct ThunderStrikeConfig : IComponentData
{
    public Entity mainAbilityPrefab;
    public float maxAftermathDisplayTime;
    public int maxStrikes;
    public float timeBetweenStrikes;
    public Entity projectileAbilityPrefab;
    public float damageDelayTime;
    public float damageArea;
    public float initialStrikeDelay;
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ThunderStrikeConfigAuthoring : MonoBehaviour
{
    public GameObject abilityPrefab;
    public float maxDisplayTime;
    public float damageDelayTime;
    public float damage;
    public float maxArea;
    public int maxStrikes;
    public float timeBetweenStrikes;

    public class ThunderStrikeConfigAuthoringBaker : Baker<ThunderStrikeConfigAuthoring>
    {
        public override void Bake(ThunderStrikeConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ThunderStrikeConfig
                {
                    abilityPrefab = GetEntity(authoring.abilityPrefab, TransformUsageFlags.Dynamic),
                    maxDisplayTime = authoring.maxDisplayTime,
                    damageDelayTime = authoring.damageDelayTime,
                    damage = authoring.damage,
                    maxArea = authoring.maxArea,
                    maxStrikes = authoring.maxStrikes,
                    timeBetweenStrikes = authoring.timeBetweenStrikes
                });
        }
    }
}

public struct ThunderStrikeConfig : IComponentData
{
    public Entity abilityPrefab;
    public float maxDisplayTime;
    public float damageDelayTime;
    public float damage;
    public float maxArea;
    public int maxStrikes;
    public float timeBetweenStrikes;
}

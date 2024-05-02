using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ThunderBoltConfigAuthoring : MonoBehaviour
{
    public GameObject abilityPrefab;
    public float maxDisplayTime;
    public float damageDelayTime;
    public float damage;
    public float maxArea;
    public int maxCount;

    public class ThunderBoltConfigAuthoringBaker : Baker<ThunderBoltConfigAuthoring>
    {
        public override void Bake(ThunderBoltConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ThunderBoltConfig
                {
                    AbilityPrefab = GetEntity(authoring.abilityPrefab, TransformUsageFlags.Dynamic),
                    MaxDisplayTime = authoring.maxDisplayTime,
                    DamageDelayTime = authoring.damageDelayTime,
                    Damage = authoring.damage,
                    MaxArea = authoring.maxArea,
                    MaxCount = authoring.maxCount
                });
        }
    }
}

public struct ThunderBoltConfig : IComponentData
{
    public Entity AbilityPrefab;
    public float MaxDisplayTime;
    public float DamageDelayTime;
    public float Damage;
    public float MaxArea;
    public int MaxCount;
}

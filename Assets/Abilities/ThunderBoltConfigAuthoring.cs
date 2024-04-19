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

    public class ThunderBoltConfigAuthoringBaker : Baker<ThunderBoltConfigAuthoring>
    {
        public override void Bake(ThunderBoltConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ThunderBoltConfig
                {
                    abilityPrefab = GetEntity(authoring.abilityPrefab, TransformUsageFlags.Dynamic),
                    maxDisplayTime = authoring.maxDisplayTime,
                    damageDelayTime = authoring.damageDelayTime,
                    damage = authoring.damage,
                    maxArea = authoring.maxArea
                });
        }
    }
}

public struct ThunderBoltConfig : IComponentData
{
    public Entity abilityPrefab;
    public float maxDisplayTime;
    public float damageDelayTime;
    public float damage;
    public float maxArea;
}

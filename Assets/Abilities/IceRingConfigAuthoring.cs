using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class IceRingConfigAuthoring : MonoBehaviour
{
    public GameObject abilityPrefab;
    public GameObject targetAreaPrefab;
    [HideInInspector] public float maxDisplayTime;
    public float maxChargeTime;
    public float damage;
    public float maxArea;
    public bool isAbilityReleased;

    public class IceRingConfigAuthoringBaker : Baker<IceRingConfigAuthoring>
    {
        public override void Bake(IceRingConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new IceRingConfig
                {
                    abilityPrefab = GetEntity(authoring.abilityPrefab, TransformUsageFlags.Dynamic),
                    targetAreaPrefab = GetEntity(authoring.targetAreaPrefab, TransformUsageFlags.Dynamic),
                    maxDisplayTime = authoring.maxDisplayTime,
                    maxChargeTime = authoring.maxChargeTime,
                    damage = authoring.damage,
                    maxArea = authoring.maxArea,
                    isAbilityReleased = authoring.isAbilityReleased
                });
        }
    }
}

public struct IceRingConfig : IComponentData
{
    public Entity abilityPrefab;
    public Entity targetAreaPrefab;
    public float maxDisplayTime;
    public float maxChargeTime;
    public float damage;
    public float maxArea;
    public bool isAbilityReleased;
}

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
    public float maxChargeTime;
    public float damage;
    public float maxArea;
    [HideInInspector] public bool isAbilityReleased;

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
    public Entity chargeAreaPrefab;
    public float maxDisplayTime;
    public float damageDelayTime;
    public float maxChargeTime;
    public float damage;
    public float maxArea;
    public bool isAbilityReleased;
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class IceRingConfigAuthoring : MonoBehaviour
{
    public GameObject abilityPrefab;
    public float maxTime;
    public float maxChargeTime;
    public float damage;
    public float maxArea;

    public class IceRingConfigAuthoringBaker : Baker<IceRingConfigAuthoring>
    {
        public override void Bake(IceRingConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new IceRingConfig
                    {
                        abilityPrefab = GetEntity(authoring.abilityPrefab, TransformUsageFlags.Dynamic)
                    });
        }
    }
}

public struct IceRingConfig : IComponentData
{
    public Entity abilityPrefab;
}

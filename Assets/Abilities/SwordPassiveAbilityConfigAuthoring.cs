using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SwordPassiveAbilityConfigAuthoring : MonoBehaviour
{
    public float radius;
    public float maxRangeFromPlayer;

    public class SwordPassiveAbilityConfigBaker : Baker<SwordPassiveAbilityConfigAuthoring>
    {
        public override void Bake(SwordPassiveAbilityConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SwordPassiveAbilityConfig { Radius = authoring.radius, MaxRangeFromPlayer = authoring.maxRangeFromPlayer});
        }
    }
}

public struct SwordPassiveAbilityConfig : IComponentData
{
    public float Radius;
    public float MaxRangeFromPlayer;
}

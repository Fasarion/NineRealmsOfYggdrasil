using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SwordPassiveAbilityConfigAuthoring : MonoBehaviour
{
    public float radius;

    public class SwordPassiveAbilityConfigBaker : Baker<SwordPassiveAbilityConfigAuthoring>
    {
        public override void Bake(SwordPassiveAbilityConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SwordPassiveAbilityConfig { Radius = authoring.radius });
        }
    }
}

public struct SwordPassiveAbilityConfig : IComponentData
{
    public float Radius;
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ThunderStrikeAbilityAuthoring : MonoBehaviour
{
    public float test;

    public class ThunderStrikeAbilityAuthoringBaker : Baker<ThunderStrikeAbilityAuthoring>
    {
        public override void Bake(ThunderStrikeAbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ThunderStrikeAbility { test = authoring.test });
        }
    }
}

public struct ThunderStrikeAbility : IComponentData
{
    public float test;
}

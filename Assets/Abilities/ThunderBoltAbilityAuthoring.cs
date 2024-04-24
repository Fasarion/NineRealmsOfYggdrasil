using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ThunderBoltAbilityAuthoring : MonoBehaviour
{
    public bool hasFired;

    public class ThunderBoltAbilityAuthoringBaker : Baker<ThunderBoltAbilityAuthoring>
    {
        public override void Bake(ThunderBoltAbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ThunderBoltAbility { hasFired = authoring.hasFired });
        }
    }
}

public struct ThunderBoltAbility : IComponentData
{
    public bool hasFired;
}

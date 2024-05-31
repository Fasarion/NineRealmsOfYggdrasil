using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HammerSpecialAbilityAuthoring : MonoBehaviour
{
    public class HammerSpecialAbilityAuthoringBaker : Baker<HammerSpecialAbilityAuthoring>
    {
        public override void Bake(HammerSpecialAbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HammerSpecialAbility());
        }
    }
}

public struct HammerSpecialAbility : IComponentData
{
}

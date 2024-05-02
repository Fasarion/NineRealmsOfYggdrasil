using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ThunderBoltAbilityAuthoring : MonoBehaviour
{
    public int currentCount;
    public bool isInitialized;

    public class ThunderBoltAbilityAuthoringBaker : Baker<ThunderBoltAbilityAuthoring>
    {
        public override void Bake(ThunderBoltAbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ThunderBoltAbility { CurrentCount = authoring.currentCount, isInitialized = authoring.isInitialized});
            AddBuffer<TargetBufferElement>(entity);
        }
    }
}

public struct ThunderBoltAbility : IComponentData
{
    public int CurrentCount;
    public bool isInitialized;
}

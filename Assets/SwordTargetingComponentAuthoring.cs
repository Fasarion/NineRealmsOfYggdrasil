using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SwordTargetingComponentAuthoring : MonoBehaviour
{
    public float offset = 0.5f;
    public bool shouldWaitForDeath;

    public class SwordBakerAuthoringBaker : Baker<SwordTargetingComponentAuthoring>
    {
        public override void Bake(SwordTargetingComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SwordTargetingComponent { Offset = authoring.offset, ShouldWaitForDeath = authoring.shouldWaitForDeath});
        }
    }
}

public struct SwordTargetingComponent : IComponentData
{
    public float Offset;
    public Entity EntityToFollow;
    public bool ShouldWaitForDeath;
}
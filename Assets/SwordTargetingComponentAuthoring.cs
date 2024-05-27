using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SwordTargetingComponentAuthoring : MonoBehaviour
{
    public float3 offset;
    public bool shouldWaitForDeath;

    public class ObjectiveObjectMarkerAuthoringBaker : Baker<ObjectiveObjectMarkerAuthoring>
    {
        public override void Bake(ObjectiveObjectMarkerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SwordTargetingComponent { Offset = authoring.offset, ShouldWaitForDeath = authoring.shouldWaitForDeath});
        }
    }
}

public struct SwordTargetingComponent : IComponentData
{
    public float3 Offset;
    public Entity EntityToFollow;
    public bool ShouldWaitForDeath;
}
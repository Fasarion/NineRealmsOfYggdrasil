using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ObjectiveObjectMarkerAuthoring : MonoBehaviour
{
    public float offset;
    public bool shouldWaitForDeath;

    public class ObjectiveObjectMarkerAuthoringBaker : Baker<ObjectiveObjectMarkerAuthoring>
    {
        public override void Bake(ObjectiveObjectMarkerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ObjectiveObjectMarkerComponent { Offset = authoring.offset, ShouldWaitForDeath = authoring.shouldWaitForDeath});
        }
    }
}

public struct ObjectiveObjectMarkerComponent : IComponentData
{
    public float Offset;
    public Entity EntityToFollow;
    public bool ShouldWaitForDeath;
}

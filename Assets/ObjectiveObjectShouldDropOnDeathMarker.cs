using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ObjectiveObjectShouldDropOnDeathMarker : MonoBehaviour
{
    public class ObjectiveObjectShouldDropOnDeathMarkerBaker : Baker<ObjectiveObjectShouldDropOnDeathMarker>
    {
        public override void Bake(ObjectiveObjectShouldDropOnDeathMarker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ObjectiveObjectShouldDropOnDeathMarkerTag());
        }
    }
}

public struct ObjectiveObjectShouldDropOnDeathMarkerTag : IComponentData
{
}

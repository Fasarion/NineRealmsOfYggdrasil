using System.Collections;
using System.Collections.Generic;
using Destruction;
using Movement;
using Unity.Entities;
using UnityEngine;

public class ObjectiveObjectAuthoring : MonoBehaviour
{
    public ObjectiveObjectType type;

    public class ObjectiveObjectAuthoringBaker : Baker<ObjectiveObjectAuthoring>
    {
        public override void Bake(ObjectiveObjectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ObjectiveObject { type = authoring.type });
            AddComponent(entity, new DirectionComponent{});
            SetComponentEnabled<DirectionComponent>(false);
            
            AddComponent(entity, new ShouldBeDestroyed{});
            SetComponentEnabled<ShouldBeDestroyed>(false);
        }
    }
}

public struct ObjectiveObject : IComponentData
{
    public ObjectiveObjectType type;
}


using System.Collections;
using System.Collections.Generic;
using Destruction;
using Movement;
using Unity.Entities;
using UnityEngine;

public class XPObjectAuthoring : MonoBehaviour
{
    public class XpObjectAuthoringBaker : Baker<XPObjectAuthoring>
    {
        public override void Bake(XPObjectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new XpObject{});
            
            AddComponent(entity, new DirectionComponent{});
            SetComponentEnabled<DirectionComponent>(false);
            
            AddComponent(entity, new ShouldBeDestroyed{});
            SetComponentEnabled<ShouldBeDestroyed>(false);
        }
    }
}

public struct XpObject : IComponentData
{
}

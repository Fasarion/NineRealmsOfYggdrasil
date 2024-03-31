using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class XPObjectAuthoring : MonoBehaviour
{
    public class XpObjectAuthoringBaker : Baker<XPObjectAuthoring>
    {
        public override void Bake(XPObjectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new XpObject{});
        }
    }
}

public struct XpObject : IComponentData
{
}

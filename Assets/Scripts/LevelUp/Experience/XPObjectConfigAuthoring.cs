using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Destruction;

public class XPObjectConfigAuthoring : MonoBehaviour
{
    public GameObject xPObjectPrefab;

    public class XpObjectConfigAuthoringBaker : Baker<XPObjectConfigAuthoring>
    {
        public override void Bake(XPObjectConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new XpObjectConfig
                    {
                        xPObjectPrefab = GetEntity(authoring.xPObjectPrefab, TransformUsageFlags.Dynamic)
                    });
        }
    }
}

public struct XpObjectConfig : IComponentData
{
    public Entity xPObjectPrefab;
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Destruction;

public class XPObjectConfigAuthoring : MonoBehaviour
{ 
    [Header("--Tweakable Values--")]
    [Tooltip("The base distance the player must be from the object for it to be picked up.")]
    public float baseDistance = 1;

    [Tooltip("How fast the object should move towards the player when the player is close enough")]
    public float moveSpeed = 1;

    public class XpObjectConfigAuthoringBaker : Baker<XPObjectConfigAuthoring>
    {
        public override void Bake(XPObjectConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new XPObjectConfig
                    {
                        baseDistance = authoring.baseDistance,
                        moveSpeed = authoring.moveSpeed
                    });
        }
    }
}

public struct XPObjectConfig : IComponentData
{
    public float baseDistance;
    public float moveSpeed;
}

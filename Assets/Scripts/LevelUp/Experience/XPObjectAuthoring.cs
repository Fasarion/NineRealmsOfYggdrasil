using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class XPObjectAuthoring : MonoBehaviour
{
    [Header("--Tweakable Values--")]
    [Tooltip("The experience gained when the object is picked up by the player.")]
    public int experience = 1;

    [Tooltip("The base distance the player must be from the object for it to be picked up.")]
    public float baseDistance = 1;
    
    [Tooltip("How fast the object should move towards ")]

    public class XpObjectAuthoringBaker : Baker<XPObjectAuthoring>
    {
        public override void Bake(XPObjectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new XpObject { experience = authoring.experience, baseDistance = authoring.baseDistance });
        }
    }
}

public struct XpObject : IComponentData
{
    public int experience;
    public float baseDistance;
}

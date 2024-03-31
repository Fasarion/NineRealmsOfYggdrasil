using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Destruction;

public class XPObjectConfigAuthoring : MonoBehaviour
{
    public GameObject xPObjectPrefab;

    [Header("--Tweakable Values--")] 
    [Tooltip("The experience gained when the object is picked up by the player.")]
    public int experience = 1;

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
                        xPObjectPrefab = GetEntity(authoring.xPObjectPrefab, TransformUsageFlags.Dynamic),
                        experience = authoring.experience,
                        baseDistance = authoring.baseDistance,
                        moveSpeed = authoring.moveSpeed
                    });
        }
    }
}

public struct XPObjectConfig : IComponentData
{
    public Entity xPObjectPrefab;
    public int experience;
    public float baseDistance;
    public float moveSpeed;
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ObjectiveObjectConfigAuthoring : MonoBehaviour
{
    [Tooltip("The base distance the player must be from the object for it to be picked up.")]
    public float baseDistance = 1;

    [Tooltip("How fast the object should move towards the player when the player is close enough")]
    public float moveSpeed = 1;

    public class ObjectiveObjectConfigAuthoringBaker : Baker<ObjectiveObjectConfigAuthoring>
    {
        public override void Bake(ObjectiveObjectConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ObjectiveObjectConfig
                    {
                        baseDistance = authoring.baseDistance, moveSpeed = authoring.moveSpeed
                    });
            var buffer = AddBuffer<ObjectivePickupBufferElement>(entity);
        }
    }
}

public struct ObjectiveObjectConfig : IComponentData
{
    public float baseDistance;
    public float moveSpeed;
}

public struct ObjectivePickupBufferElement : IBufferElementData
{
    public ObjectiveObjectType Value;
}
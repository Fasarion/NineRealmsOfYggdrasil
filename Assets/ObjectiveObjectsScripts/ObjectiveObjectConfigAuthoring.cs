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

    [Tooltip("How many seconds that will elapse between objective object spawns.")]
    public float spawnRate = 10;

    public GameObject objectiveObjectPrefab;

    public GameObject objectiveObjectMarkerPrefab;

    public float markerOffset = 3;

    public class ObjectiveObjectConfigAuthoringBaker : Baker<ObjectiveObjectConfigAuthoring>
    {
        public override void Bake(ObjectiveObjectConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ObjectiveObjectConfig
                {
                    BaseDistance = authoring.baseDistance,
                    MoveSpeed = authoring.moveSpeed,
                    SpawnRate = authoring.spawnRate,
                    ObjectiveObjectPrefab = GetEntity(authoring.objectiveObjectPrefab, TransformUsageFlags.Dynamic),
                    ObjectiveObjectMarkerPrefab =
                        GetEntity(authoring.objectiveObjectMarkerPrefab, TransformUsageFlags.Dynamic),
                    MarkerOffset = authoring.markerOffset
                });
            AddBuffer<ObjectivePickupBufferElement>(entity);
        }
    }
}

public struct ObjectiveObjectConfig : IComponentData
{
    public float BaseDistance;
    public float MoveSpeed;
    public float SpawnRate;
    public Entity ObjectiveObjectPrefab;
    public Entity ObjectiveObjectMarkerPrefab;
    public float MarkerOffset;
}

public struct ObjectivePickupBufferElement : IBufferElementData
{
    public ObjectiveObjectType Value;
}
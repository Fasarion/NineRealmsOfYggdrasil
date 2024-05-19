using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DestructibleObjectSpawningComponentAuthoring : MonoBehaviour
{
    public GameObject objectToSpawn;

    public class DestructibleObjectSpawningComponentAuthoringBaker : Baker<DestructibleObjectSpawningComponentAuthoring>
    {
        public override void Bake(DestructibleObjectSpawningComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new DestructibleObjectSpawningComponent
                {
                    ObjectToSpawn = GetEntity(authoring.objectToSpawn, TransformUsageFlags.Dynamic)
                });
        }
    }
}

public struct DestructibleObjectSpawningComponent : IComponentData
{
    public Entity ObjectToSpawn;
}

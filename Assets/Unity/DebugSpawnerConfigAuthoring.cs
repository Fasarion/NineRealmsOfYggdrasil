using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class DebugSpawnerConfigAuthoring : MonoBehaviour
{
    public GameObject objectToSpawn;
    public int numberOfObjectsToSpawnSquare;
    public float3 spawnPointStart;
    public bool spawn;

    public class DebugSpawnerConfigAuthoringBaker : Baker<DebugSpawnerConfigAuthoring>
    {
        public override void Bake(DebugSpawnerConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new DebugSpawnerConfig
                    {
                        objectToSpawn = GetEntity(authoring.objectToSpawn, TransformUsageFlags.Dynamic),
                        numberOfObjectsToSpawnSquare = authoring.numberOfObjectsToSpawnSquare,
                        spawnPointStart = authoring.spawnPointStart,
                        spawn = authoring.spawn
                    });
        }
    }
}

public struct DebugSpawnerConfig : IComponentData
{
    public Entity objectToSpawn;
    public int numberOfObjectsToSpawnSquare;
    public float3 spawnPointStart;
    public bool spawn;
}

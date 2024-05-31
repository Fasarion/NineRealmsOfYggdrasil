using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnEntityOnDestroyAuthoring : MonoBehaviour
{
    [Header("Spawn Objects")]
    [FormerlySerializedAs("objectsWithSettings")]
    [FormerlySerializedAs("spawnObjects")]
    [Tooltip("Game Objects to spawn when this entity gets destroyed (with settings).")]
    [SerializeField] private List<GameObject> entitiesToSpawn;

    [Header("Spawn Settings")]
    [Tooltip("Settings for entities to spawn")]
    [SerializeField] private SpawnSettings spawnSettings;
    
    // [Tooltip("Game Objects to spawn when this entity gets destroyed (without settings).")]
    // [SerializeField] private List<GameObject> objectsWithOutSettings;

    private void OnValidate()
    {
        if (spawnSettings.NewScale <= 0)
        {
            spawnSettings.NewScale = 1;
        }
    }

    class Baker : Baker<SpawnEntityOnDestroyAuthoring>
    {
        public override void Bake(SpawnEntityOnDestroyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            var spawnBuffer = AddBuffer<SpawnEntityOnDestroyElement>(entity);
            foreach (var spawnSettingsObject in authoring.entitiesToSpawn)
            {
                spawnBuffer.Add(new SpawnEntityOnDestroyElement
                {
                    Value = GetEntity(spawnSettingsObject, TransformUsageFlags.Dynamic),
                });
            }
            
            AddComponent(entity, new SpawnSettingsComponent{Value = authoring.spawnSettings});
            AddComponent(entity, new SpawnOnDestroyTag());
            
            // var normalBuffer = AddBuffer<SpawnNormalEntityOnDestroyElement>(entity);
            // foreach (var spawnObject in authoring.objectsWithOutSettings)
            // {
            //     normalBuffer.Add(new SpawnNormalEntityOnDestroyElement
            //     {
            //         Value = GetEntity(spawnObject, TransformUsageFlags.Dynamic),
            //     });
            // }
        }
    }
}

public struct SpawnOnDestroyTag : IComponentData{}

public struct SpawnSettingsComponent : IComponentData
{
    public SpawnSettings Value;
}
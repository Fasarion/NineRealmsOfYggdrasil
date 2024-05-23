using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnEntityOnDestroyAuthoring : MonoBehaviour
{
    [Tooltip("Game Objects to spawn when this entity gets destroyed.")]
    [SerializeField] private List<SpawnObjectContents> spawnObjects;

    private void OnValidate()
    {
        for (var i = 0; i < spawnObjects.Count; i++)
        {
            var spawnObject = spawnObjects[i];
            if (spawnObject.SpawnSettings.NewScale <= 0)
            {
                spawnObject.SpawnSettings.NewScale = 1;
                spawnObjects[i] = spawnObject;
            }
        }
    }

    class Baker : Baker<SpawnEntityOnDestroyAuthoring>
    {
        public override void Bake(SpawnEntityOnDestroyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            var buffer = AddBuffer<SpawnEntityOnDestroyElement>(entity);
            foreach (var spawnObject in authoring.spawnObjects)
            {
                buffer.Add(new SpawnEntityOnDestroyElement
                {
                    Entity = GetEntity(spawnObject.SpawnPrefab, TransformUsageFlags.Dynamic),
                    Settings = spawnObject.SpawnSettings
                });
            }
        }
    }
}
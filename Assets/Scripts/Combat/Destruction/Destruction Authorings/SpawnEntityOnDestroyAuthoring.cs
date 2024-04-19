using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnEntityOnDestroyAuthoring : MonoBehaviour
{
    [Tooltip("Game Objects to spawn when this entity gets destroyed.")]
    [SerializeField] private List<GameObject> spawnObjects;

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
                    Value = GetEntity(spawnObject, TransformUsageFlags.Dynamic), 
                });
            }
        }
    }
}
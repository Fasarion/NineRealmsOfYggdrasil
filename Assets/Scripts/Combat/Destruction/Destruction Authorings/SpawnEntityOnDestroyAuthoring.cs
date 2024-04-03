using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnEntityOnDestroyAuthoring : MonoBehaviour
{
    [Tooltip("Game Object to spawn when this entity gets destroyed.")]
    [SerializeField] private GameObject spawnObject;

    class Baker : Baker<SpawnEntityOnDestroyAuthoring>
    {
        public override void Bake(SpawnEntityOnDestroyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
            new SpawnEntityOnDestroy
            {
                Value = GetEntity(authoring.spawnObject, TransformUsageFlags.Dynamic)
            });
        }
    }
}
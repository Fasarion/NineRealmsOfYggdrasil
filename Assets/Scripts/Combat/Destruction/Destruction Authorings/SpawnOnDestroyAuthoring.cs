using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnOnDestroyAuthoring : MonoBehaviour
{
    [Tooltip("Game Object to spawn when this entity gets destroyed.")]
    [SerializeField] private GameObject spawnObject;

    class Baker : Baker<SpawnOnDestroyAuthoring>
    {
        public override void Bake(SpawnOnDestroyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            // AddComponent(entity, new CapabilityPrefab
            // {
            //     Value = GetEntity(authoring.CapabilityPrefab, TransformUsageFlags.Dynamic)
            // });
        }
    }
}

// public struct SpawnEntityOnDestroy

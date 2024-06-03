using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SwordProjectileAbilityAuthoring : MonoBehaviour
{
    public int currentSpawnCount;
    public bool isInitialized;
    public int bufferLength;

    public class SwordProjectileAbilityAuthoringBaker : Baker<SwordProjectileAbilityAuthoring>
    {
        public override void Bake(SwordProjectileAbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new SwordProjectileAbility
                {
                    CurrentSpawnCount = authoring.currentSpawnCount, IsInitialized = authoring.isInitialized,
                    bufferLength = authoring.bufferLength,
                });
        }
    }
}

public struct SwordProjectileAbility : IComponentData
{
    public int CurrentSpawnCount;
    public bool IsInitialized;
    public int bufferLength;
}

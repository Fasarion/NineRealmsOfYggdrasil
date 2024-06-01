using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class HammerSpecialAbilityAuthoring : MonoBehaviour
{
    public bool hasFired;
    public bool hasWaitedForInitialThrow;
    public int currentSpawnCount;

    public class HammerSpecialAbilityAuthoringBaker : Baker<HammerSpecialAbilityAuthoring>
    {
        public override void Bake(HammerSpecialAbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new HammerSpecialAbility
                {
                    HasFired = authoring.hasFired, HasWaitedForInitialThrow = authoring.hasWaitedForInitialThrow,
                    CurrentSpawnCount = authoring.currentSpawnCount,
                });
        }
    }
}

public struct HammerSpecialAbility : IComponentData
{
    public bool HasFired;
    public bool HasWaitedForInitialThrow;
    public int CurrentSpawnCount;
}

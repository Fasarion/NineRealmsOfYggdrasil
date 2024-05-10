using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HammerPassiveAbilityAuthoring : MonoBehaviour
{
    public int currentStrikeCheckpoint;
    public bool hasFired;
    public float3 ogStrikePosition;

    public class HammerPassiveAbilityAuthoringBaker : Baker<HammerPassiveAbilityAuthoring>
    {
        public override void Bake(HammerPassiveAbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new HammerPassiveAbility
                {
                    CurrentStrikeCheckpoint = authoring.currentStrikeCheckpoint,
                    HasFired = authoring.hasFired,
                    OgStrikePosition = authoring.ogStrikePosition
                });
        }
    }
}

public struct HammerPassiveAbility : IComponentData
{
    public int CurrentStrikeCheckpoint;
    public bool HasFired;
    public float3 OgStrikePosition;
}

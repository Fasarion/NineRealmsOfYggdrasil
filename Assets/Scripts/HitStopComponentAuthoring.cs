using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HitStopComponentAuthoring : MonoBehaviour
{
    public float currentElapsedTime;
    public float maxDuration;

    public class HitStopComponentAuthoringBaker : Baker<HitStopComponentAuthoring>
    {
        public override void Bake(HitStopComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new HitStopComponent
                {
                    CurrentElapsedTime = authoring.currentElapsedTime, MaxDuration = authoring.maxDuration
                });
        }
    }
}

public struct HitStopComponent : IComponentData
{
    public float CurrentElapsedTime;
    public float MaxDuration;
}

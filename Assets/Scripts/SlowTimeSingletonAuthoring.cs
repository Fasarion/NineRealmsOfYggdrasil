using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SlowTimeSingletonAuthoring : MonoBehaviour
{
    public bool isTimeSlowed;
    public float currentSlowFactor;

    public class SlowTimeSingletonAuthoringBaker : Baker<SlowTimeSingletonAuthoring>
    {
        public override void Bake(SlowTimeSingletonAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new SlowTimeSingleton
                {
                    IsTimeSlowed = authoring.isTimeSlowed, CurrentSlowFactor = authoring.currentSlowFactor
                });
        }
    }
}

public struct SlowTimeSingleton : IComponentData
{
    public bool IsTimeSlowed;
    public float CurrentSlowFactor;
}

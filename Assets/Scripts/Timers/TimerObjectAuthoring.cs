using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TimerObjectAuthoring : MonoBehaviour
{
    public float currentTime;
    public float maxTime;

    public class TimerObjectAuthoringBaker : Baker<TimerObjectAuthoring>
    {
        public override void Bake(TimerObjectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new TimerObject { currentTime = authoring.currentTime, maxTime = authoring.maxTime });
        }
    }
}

public struct TimerObject : IComponentData
{
    public float currentTime;
    public float maxTime;
}

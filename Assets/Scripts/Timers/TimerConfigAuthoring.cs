using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TimerConfigAuthoring : MonoBehaviour
{
    public GameObject timerObject;

    public class TimerConfigAuthoringBaker : Baker<TimerConfigAuthoring>
    {
        public override void Bake(TimerConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new TimerConfig
                    {
                        timerObject = GetEntity(authoring.timerObject, TransformUsageFlags.None)
                    });
        }
    }
}

public struct TimerConfig : IComponentData
{
    public Entity timerObject;
}

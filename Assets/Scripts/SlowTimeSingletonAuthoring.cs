using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SlowTimeSingletonAuthoring : MonoBehaviour
{
    public bool isTimeSlowed;
    public float currentSlowFactor;
    public float slowFactorTarget;
    public float fadeInSpeed;
    public float fadeOutSpeed;
    public bool shouldSlowTime;
    public float slowTargetDuration;

    public class SlowTimeSingletonAuthoringBaker : Baker<SlowTimeSingletonAuthoring>
    {
        public override void Bake(SlowTimeSingletonAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new SlowTimeSingleton
                {
                    IsTimeSlowed = authoring.isTimeSlowed,
                    CurrentSlowFactor = authoring.currentSlowFactor,
                    SlowFactorTarget = authoring.slowFactorTarget,
                    FadeInSpeed = authoring.fadeInSpeed,
                    FadeOutSpeed = authoring.fadeOutSpeed,
                    ShouldSlowTime = authoring.shouldSlowTime,
                    SlowTargetDuration = authoring.slowTargetDuration,
                });
        }
    }
}

public struct SlowTimeSingleton : IComponentData
{
    public bool IsTimeSlowed;
    public float CurrentSlowFactor;
    public float SlowFactorTarget;
    public float FadeInSpeed;
    public float FadeOutSpeed;
    public bool ShouldSlowTime;
    public float SlowTargetDuration;
}
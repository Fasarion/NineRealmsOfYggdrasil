using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ShouldSlowTimeComponentAuthoring : MonoBehaviour
{
    public float slowFactor;
    public float fadeInTime;
    public float fadeOutTime;
    public float slowDuration;

    public class ShouldSlowTimeComponentAuthoringBaker : Baker<ShouldSlowTimeComponentAuthoring>
    {
        public override void Bake(ShouldSlowTimeComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ShouldSlowTimeComponent
                {
                    SlowFactor = authoring.slowFactor,
                    FadeInTime = authoring.fadeInTime,
                    FadeOutTime = authoring.fadeOutTime,
                    SlowDuration = authoring.slowDuration
                });
        }
    }
}

public struct ShouldSlowTimeComponent : IComponentData
{
    public float SlowFactor;
    public float FadeInTime;
    public float FadeOutTime;
    public float SlowDuration;
}

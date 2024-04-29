using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ShouldApplyHitStopOnHitAuthoring : MonoBehaviour
{
    public float duration;

    public class ShouldApplyHitStopOnHitAuthoringBaker : Baker<ShouldApplyHitStopOnHitAuthoring>
    {
        public override void Bake(ShouldApplyHitStopOnHitAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShouldApplyHitStopOnHit { Duration = authoring.duration });
        }
    }
}

public struct ShouldApplyHitStopOnHit : IComponentData
{
    public float Duration;
}

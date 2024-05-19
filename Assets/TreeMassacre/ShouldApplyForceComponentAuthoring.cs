using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ShouldApplyForceComponentAuthoring : MonoBehaviour
{
    public class ShouldApplyForceComponentAuthoringBaker : Baker<ShouldApplyForceComponentAuthoring>
    {
        public override void Bake(ShouldApplyForceComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShouldApplyForceComponent());
        }
    }
}

public struct ShouldApplyForceComponent : IComponentData
{
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ShouldHaveForceAppliedComponentAuthoring : MonoBehaviour
{
    public class ShouldHaveForceAppliedComponentAuthoringBaker : Baker<ShouldHaveForceAppliedComponentAuthoring>
    {
        public override void Bake(ShouldHaveForceAppliedComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShouldHaveForceAppliedComponent());
        }
    }
}

public struct ShouldHaveForceAppliedComponent : IComponentData
{
}

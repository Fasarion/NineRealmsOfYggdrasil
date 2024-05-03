using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawningEnabledComponentAuthoring : MonoBehaviour
{
    public class SpawningEnabledComponentAuthoringBaker : Baker<SpawningEnabledComponentAuthoring>
    {
        public override void Bake(SpawningEnabledComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawningEnabledComponent());
        }
    }
}

public struct SpawningEnabledComponent : IComponentData
{
}

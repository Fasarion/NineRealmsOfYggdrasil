using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnCountAuthoring : MonoBehaviour
{
    public int value;

    public class SpawnCountAuthoringBaker : Baker<SpawnCountAuthoring>
    {
        public override void Bake(SpawnCountAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawnCount { Value = authoring.value });
        }
    }
}

public struct SpawnCount : IComponentData
{
    public int Value;
}

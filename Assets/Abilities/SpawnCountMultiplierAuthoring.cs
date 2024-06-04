using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnCountMultiplierAuthoring : MonoBehaviour
{
    public int value;

    public class SpawnCountMultiplierAuthoringBaker : Baker<SpawnCountMultiplierAuthoring>
    {
        public override void Bake(SpawnCountMultiplierAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawnCountMultiplier { Value = authoring.value });
        }
    }
}

public struct SpawnCountMultiplier : IComponentData
{
    public int Value;
}

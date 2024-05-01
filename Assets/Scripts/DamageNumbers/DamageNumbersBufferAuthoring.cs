using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class DamageNumbersBufferAuthoring : MonoBehaviour
{
    public class DamageNumbersBufferAuthoringBaker : Baker<DamageNumbersBufferAuthoring>
    {
        public override void Bake(DamageNumbersBufferAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DamageNumbersBufferComponentData());
            AddBuffer<DamageNumberBufferElement>(entity);
        }
    }
}

public struct DamageNumbersBufferComponentData : IComponentData
{
}

public struct DamageNumberBufferElement : IBufferElementData
{
    public bool isCritical;
    public float damage;
    public float3 position;
}

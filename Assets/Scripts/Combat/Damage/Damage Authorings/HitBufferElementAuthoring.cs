using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Damage
{
    public class HitBufferElementAuthoring : MonoBehaviour
    {
        public class Baker : Baker<HitBufferElementAuthoring>
        {
            public override void Bake(HitBufferElementAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddBuffer<HitBufferElement>(entity);
            }
        }
    }
    
    public struct HitBufferElement : IBufferElementData
    {
        public bool IsHandled;
        public float3 Position;
        public float2 Normal;
        public Entity HitEntity;
    }
}


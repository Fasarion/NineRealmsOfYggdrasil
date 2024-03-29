using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Damage
{
    public class HitBufferElementAuthoring : MonoBehaviour
    {
        public class CapabilityBaker : Baker<HitBufferElementAuthoring>
        {
            public override void Bake(HitBufferElementAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddBuffer<HitBufferElement>(entity);
            }
        }
    }
}


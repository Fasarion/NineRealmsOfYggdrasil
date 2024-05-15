using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Damage
{
    public class KnockBackBufferElementAuthoring : MonoBehaviour
    {
        public class CapabilityBaker : Baker<KnockBackBufferElementAuthoring>
        {
            public override void Bake(KnockBackBufferElementAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddBuffer<KnockBackBufferElement>(entity);
            }
        }
    }
    
    public struct KnockBackBufferElement : IBufferElementData
    {
        public float3 KnockBackForce;
    }
}
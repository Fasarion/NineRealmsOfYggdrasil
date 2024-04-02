using Unity.Entities;
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
}
using Unity.Entities;
using Unity.Mathematics;

namespace Damage
{
    public struct KnockBackBufferElement : IBufferElementData
    {
        public float2 KnockBackForce;
    }
}
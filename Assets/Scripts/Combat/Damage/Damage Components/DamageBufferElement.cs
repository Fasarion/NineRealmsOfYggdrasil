using Unity.Entities;

namespace Damage
{
    public struct DamageBufferElement : IBufferElementData
    {
        public DamageContents DamageContents;
    }
}
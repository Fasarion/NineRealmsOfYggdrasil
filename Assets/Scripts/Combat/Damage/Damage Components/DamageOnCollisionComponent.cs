using Unity.Entities;

namespace Damage
{
    public struct DamageOnCollisionComponent : IComponentData
    {
        public DamageContents DamageContents;
    }
}

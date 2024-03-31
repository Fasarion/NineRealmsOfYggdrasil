using Unity.Entities;

namespace Damage
{
    public struct DamageFlashTimer : IComponentData, IEnableableComponent
    {
        public float FlashTime;
        public float CurrentTime;
    }
}
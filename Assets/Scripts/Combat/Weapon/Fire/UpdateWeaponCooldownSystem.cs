using Unity.Entities;

namespace Weapon
{
    public partial struct UpdateWeaponCooldownSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var weapon in SystemAPI.Query<RefRW<GunComponent>>())
            {
                weapon.ValueRW.CurrentCoolDownTime += SystemAPI.Time.DeltaTime;
                weapon.ValueRW.WantsToFire = false;
            }
        }
    }
}


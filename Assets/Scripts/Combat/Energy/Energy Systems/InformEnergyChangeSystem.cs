using Unity.Entities;

public partial class InformEnergyChangeSystem : SystemBase
{
    protected override void OnUpdate()
    {
        foreach (var (energyBar, weapon) in SystemAPI.Query<EnergyBarComponent, WeaponComponent>().
            WithAll<HasChangedEnergy>()
        )
        {
            EventManager.OnEnergyChange?.Invoke(weapon.WeaponType, energyBar.CurrentEnergy, energyBar.MaxEnergy);
        }
    }
}
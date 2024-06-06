using Unity.Entities;

public partial class InformEnergyChangeSystem : SystemBase
{
    protected override void OnStartRunning()
    {
        EventManager.OnWeaponCountSet += OnWeaponCountSet;
    }
    
    protected override void OnStopRunning()
    {
        EventManager.OnWeaponCountSet -= OnWeaponCountSet;
    }

    private void OnWeaponCountSet(int count)
    {
        Entity weapon = GetWeaponEntity(count);
        EntityManager.AddComponent<IsUnlocked>(weapon);
    }

    private Entity GetWeaponEntity(int count)
    {
        switch (count)
        {
            case 1:
                foreach (var (sword, entity) in SystemAPI.Query<SwordComponent>()
                    .WithEntityAccess())
                {
                    return entity;
                }
                break;
                
            case 2:
                foreach (var (hammer, entity) in SystemAPI.Query<HammerComponent>()
                    .WithEntityAccess())
                {
                    return entity;
                }
                break;
                
            case 3:
                foreach (var (hammer, entity) in SystemAPI.Query<BirdsComponent>()
                    .WithEntityAccess())
                {
                    return entity;
                }
                break;
        }

        return default;
    }


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
using Patrik;
using UnityEngine.Events;

public static class EventManager
{
    public static UnityAction<WeaponBehaviour, bool> OnSetupWeapon;
    public static UnityAction<WeaponBehaviour> OnWeaponSwitch;
    public static UnityAction<WeaponType, float, float> OnEnergyChange;
    public static UnityAction<AttackType> OnAttackInput;
    public static UnityAction<PlayerHealthData> OnPlayerHealthSet;
    public static UnityAction<ExperienceInfo> OnPlayerExperienceChange;

    public static UnityAction<BusyAttackInfo> OnBusyUpdate;
    public static UnityAction<int> OnChargeLevelChange;
}

public struct PlayerHealthData
{
    public float currentHealth;
    public float maxHealth;
}

using Patrik;
using UnityEngine.Events;

public static class EventManager
{
    public static UnityAction<WeaponBehaviour, bool> OnSetupWeapon;
    public static UnityAction<WeaponBehaviour> OnWeaponSwitch;

    public static UnityAction OnPause;
    public static UnityAction OnUnpause;
    
    public static UnityAction<AttackData> OnActiveAttackStart;
    public static UnityAction<AttackData> OnActiveAttackStop;
    
    public static UnityAction<AttackData> OnPassiveAttackStart;
    public static UnityAction<AttackData> OnPassiveAttackStop;
    
    public static UnityAction<WeaponType> OnWeaponActive;
    public static UnityAction<WeaponType> OnWeaponPassive;

    public static UnityAction<AttackType, bool> OnUpdateAttackAnimation;
    
    public static UnityAction<AttackData> OnSpecialCharge;
    public static UnityAction<AttackData> OnUltimatePrepare;

    public static UnityAction<WeaponType, float, float> OnEnergyChange;
    public static UnityAction<AttackType> OnAttackInput;
    public static UnityAction<PlayerHealthData> OnPlayerHealthSet;
    public static UnityAction<ExperienceInfo> OnPlayerExperienceChange;

    public static UnityAction<BusyAttackInfo> OnBusyUpdate;
    public static UnityAction<int> OnChargeLevelChange;
    
    public static UnityAction<bool> OnEnableMovementInput;
    public static UnityAction<bool> OnEnableRotationInput;
}

public struct PlayerHealthData
{
    public float currentHealth;
    public float maxHealth;
}

using System.Collections.Generic;
using Patrik;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static UnityAction<WeaponSetupData> OnSetupWeapon;
    public static UnityAction<List<WeaponSetupData>> OnAllWeaponsSetup;
    public static UnityAction<WeaponSetupData,List<WeaponSetupData>> OnWeaponSwitch;

    public static UnityAction<PauseType> OnPause;
    public static UnityAction<PauseType> OnUnpause;
    
    public static UnityAction<AttackData> OnActiveAttackStart;
    public static UnityAction<AttackData> OnActiveAttackStop;
    
    public static UnityAction<AttackData> OnPassiveAttackStart;
    public static UnityAction<AttackData> OnPassiveAttackStop;
    
    public static UnityAction<WeaponType> OnWeaponActive;
    public static UnityAction<WeaponType> OnWeaponPassive;

    public static UnityAction<AttackType, bool> OnUpdateAttackAnimation;
    
    public static UnityAction<AttackData> OnSpecialCharge;
    public static UnityAction<AttackData> OnUltimatePrepare;
    public static UnityAction<WeaponType, AttackData> OnUltimatePerform;
    

    public static UnityAction<WeaponType, float, float> OnEnergyChange;
    public static UnityAction<AttackType> OnAttackInput;
    public static UnityAction<PlayerHealthData> OnPlayerHealthSet;
    public static UnityAction<ExperienceInfo> OnPlayerExperienceChange;

    public static UnityAction<BusyAttackInfo> OnBusyUpdate;
    public static UnityAction<int> OnChargeLevelChange;
    
    public static UnityAction<bool> OnEnableMovementInput;
    public static UnityAction<bool> OnEnableRotationInput;
    public static UnityAction<bool> OnEnablePlayerInvincibility;

    public static UnityAction OnDashBegin;
    public static UnityAction OnDashInput;
    public static UnityAction OnDashEnd;
    public static UnityAction<DynamicBuffer<DashInfoElement>> OnDashInfoUpdate;

    public static UnityAction OnScreenFadeComplete;

    public static UnityAction OnPlayerDeath;

    public static UnityAction<MoveSpeedChangeData> OnChangeMoveSpeed;
    public static UnityAction OnResetMoveSpeed;
    
    public static UnityAction OnEnableUI;
    public static UnityAction OnDisableUI;

    public static UnityAction<int> OnWeaponCountSet;

    public static UnityAction<WeaponType> OnSpecialAttackUnlocked;
    
    public static UnityAction OnObjectiveReached;

    public static UnityAction<MenuButtonSelection> OnSceneChange;

    public static UnityAction<Vector3> OnPlayerMoveInput;

    public static UnityAction<float> OnPlayerDamageReductionSet;
}

public struct PlayerHealthData
{
    public float currentHealth;
    public float maxHealth;
}

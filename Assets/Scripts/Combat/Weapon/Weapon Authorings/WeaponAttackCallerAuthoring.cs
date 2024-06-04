using System.Collections;
using System.Collections.Generic;
using Patrik;
using Unity.Entities;
using UnityEngine;

public class WeaponAttackCallerAuthoring : MonoBehaviour
{
    public class WeaponAttackCallerAuthoringBaker : Baker<WeaponAttackCallerAuthoring>
    {
        public override void Bake(WeaponAttackCallerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new WeaponAttackCaller { });
        }
    }
}

public struct WeaponAttackCaller : IComponentData
{
    public WeaponCallData ActiveAttackData;
    public WeaponCallData PassiveAttackData;

    public SpecialChargeInfo SpecialChargeInfo; 
    public PrepareUltimateInfo PrepareUltimateInfo;

    public UnlockInfo UnlockInfo;

    public BusyAttackInfo BusyAttackInfo;

    public bool ReturnWeapon;

    public readonly bool AttackUnlocked(WeaponType weaponType, AttackType attackType)
    {
        return UnlockInfo.AttackUnlocked(weaponType, attackType);
    }
    
    public readonly bool IsPreparingAttack()
    {
        return SpecialChargeInfo.chargeState == ChargeState.Start || PrepareUltimateInfo.IsPreparing;
    }

    public readonly bool ShouldStartActiveAttack(WeaponType weaponType, AttackType attackType)
    {
        return ActiveAttackData.ShouldStartAttack(weaponType, attackType);
    }
    
    public readonly bool ShouldStartPassiveAttack(WeaponType type)
    {
        return PassiveAttackData.ShouldStartAttack(type, AttackType.Passive);
    }
    
    public readonly bool ShouldStopActiveAttack(WeaponType weaponType, AttackType attackType)
    {
        return ActiveAttackData.ShouldStopAttack(weaponType, attackType);
    }
    
    public readonly bool ShouldStopPassiveAttack(WeaponType type)
    {
        return PassiveAttackData.ShouldStopAttack(type, AttackType.Passive);
    }
}

public partial struct SpecialChargeInfo
{
    public ChargeState chargeState;
    public WeaponType ChargingWeapon;
    public int Level;

    public readonly bool IsChargingWithWeapon(WeaponType weaponType)
    {
        if (chargeState != ChargeState.Ongoing) return false;

        return weaponType == ChargingWeapon;
    }
}

public partial struct PrepareUltimateInfo
{
    public bool Perform;
    public bool HasPreparedThisFrame;
    public bool IsPreparing;
}

public struct UnlockInfo
{
    struct UnlockWeaponInfo
    {
        public WeaponType WeaponType;
        public bool PassiveUnlocked;
        public bool NormalUnlocked;
        public bool SpecialUnlocked;
        public bool UltimateUnlocked;

        public bool AttackUnlocked(AttackType type)
        {
            switch (type)
            {
                case AttackType.Passive: return PassiveUnlocked;
                case AttackType.Normal: return NormalUnlocked;
                case AttackType.Special: return SpecialUnlocked;
                case AttackType.Ultimate: return UltimateUnlocked;
            }
            
            Debug.LogError($"{type.ToString()} is not a recognized attack for unlock.");
            return false;
        }
    }
    
    private UnlockWeaponInfo swordInfo;
    private UnlockWeaponInfo hammerInfo;
    private UnlockWeaponInfo birdsInfo;
    
    public readonly bool AttackUnlocked(WeaponType weaponType, AttackType attackType)
    {
        switch (weaponType)
        {
            case WeaponType.Sword: return swordInfo.AttackUnlocked(attackType);
            case WeaponType.Hammer: return hammerInfo.AttackUnlocked(attackType);
            case WeaponType.Birds: return birdsInfo.AttackUnlocked(attackType);
        }
        
        Debug.LogError($"{weaponType.ToString()} is not a recognized weapon for unlock.");
        return false;
    }
}

public struct WeaponCallData
{
    public bool ShouldStart;
    public bool ShouldStop;
    public AttackType AttackType;
    public WeaponType WeaponType;
    public int Combo;
    public bool IsAttacking;

    public readonly bool ShouldStartAttack(WeaponType weaponType, AttackType attackType)
    {
        if (!ShouldStart) return false;

        return weaponType == WeaponType && attackType == AttackType;
    }
    
    public readonly bool ShouldStopAttack(WeaponType weaponType, AttackType attackType)
    {
        if (!ShouldStop) return false;

        return weaponType == WeaponType && attackType == AttackType;
    }
    
    public readonly bool ShouldStopAttack(WeaponType weaponType)
    {
        if (!ShouldStop) return false;

        return weaponType == WeaponType;
    }
}

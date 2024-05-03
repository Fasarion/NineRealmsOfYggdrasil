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

    // tempo variable, TODO: make general
    public bool ResetWeaponCurrentWeaponTransform;
    
    public readonly bool IsPreparingAttack()
    {
        return SpecialChargeInfo.chargeState == ChargeState.Start || PrepareUltimateInfo.IsPreparing;
    }

    public readonly bool ShouldStartActiveAttack(WeaponType weaponType, AttackType attackType)
    {
        return ActiveAttackData.ShouldStartAttack(weaponType, attackType);
    }
    
    public bool ShouldStartPassiveAttack(WeaponType type)
    {
        return PassiveAttackData.ShouldStartAttack(type, AttackType.Passive);
    }
    
    public bool ShouldStopActiveAttack(WeaponType weaponType, AttackType attackType)
    {
        return ActiveAttackData.ShouldStopAttack(weaponType, attackType);
    }
    
    public bool ShouldStopPassiveAttack(WeaponType type)
    {
        return PassiveAttackData.ShouldStopAttack(type, AttackType.Passive);
    }
}

public partial struct SpecialChargeInfo
{

    public ChargeState chargeState;
   // public bool IsCharging;
    public WeaponType ChargingWeapon;

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

    public bool ValidPrepareInputPressed;
}

public struct WeaponCallData
{
    public bool ShouldStart;
    public bool ShouldStop;
    public AttackType AttackType;
    public WeaponType WeaponType;
    public int Combo;
    public bool IsAttacking;

    public bool ShouldStartAttack(WeaponType weaponType, AttackType attackType)
    {
        if (!ShouldStart) return false;

        return weaponType == WeaponType && attackType == AttackType;
    }
    
    public bool ShouldStopAttack(WeaponType weaponType, AttackType attackType)
    {
        if (!ShouldStop) return false;

        return weaponType == WeaponType && attackType == AttackType;
    }
    
    public bool ShouldStopAttack(WeaponType weaponType)
    {
        if (!ShouldStop) return false;

        return weaponType == WeaponType;
    }
}

using System.Collections;
using System.Collections.Generic;
using Patrik;
using Unity.Entities;
using UnityEngine;

public class WeaponAttackCallerAuthoring : MonoBehaviour
{
    // public bool shouldActiveAttack;
    // public AttackType currentAttackType;
    // public WeaponType currentWeaponType;

    public class WeaponAttackCallerAuthoringBaker : Baker<WeaponAttackCallerAuthoring>
    {
        public override void Bake(WeaponAttackCallerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new WeaponAttackCaller
                    {
                        // shouldStartActiveAttack = authoring.shouldActiveAttack,
                        // currentStartedActiveAttackType = authoring.currentAttackType,
                        // currentStartedActiveWeaponType = authoring.currentWeaponType
                    });
        }
    }
}

public struct WeaponAttackCaller : IComponentData
{
    public WeaponCallData StartActiveAttackData;
    
    // public bool shouldStartActiveAttack;
    // public AttackType currentStartedActiveAttackType;
    // public WeaponType currentStartedActiveWeaponType;
    // public int currentStartedActiveCombo;

    public bool shouldStartPassiveAttack;
    public WeaponType currentStartedPassiveWeaponType;
    
    public bool ShouldActiveAttackWithType(WeaponType type, AttackType attackType)
    {
        if (!StartActiveAttackData.Enabled) return false;

        return type == StartActiveAttackData.WeaponType && attackType == StartActiveAttackData.AttackType;
    }
    
    public bool ShouldPassiveAttackWithType(WeaponType type)
    {
        if (!shouldStartPassiveAttack) return false;

        return type == currentStartedPassiveWeaponType;
    }
}

public struct WeaponCallData
{
    public bool Enabled;
    public AttackType AttackType;
    public WeaponType WeaponType;
    public int Combo;
}

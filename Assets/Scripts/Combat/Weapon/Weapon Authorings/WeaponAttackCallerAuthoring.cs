using System.Collections;
using System.Collections.Generic;
using Patrik;
using Unity.Entities;
using UnityEngine;

public class WeaponAttackCallerAuthoring : MonoBehaviour
{
    public bool shouldActiveAttack;
    public AttackType currentAttackType;
    public WeaponType currentWeaponType;

    public class WeaponAttackCallerAuthoringBaker : Baker<WeaponAttackCallerAuthoring>
    {
        public override void Bake(WeaponAttackCallerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new WeaponAttackCaller
                    {
                        shouldActiveAttack = authoring.shouldActiveAttack,
                        currentActiveAttackType = authoring.currentAttackType,
                        currentActiveWeaponType = authoring.currentWeaponType
                    });
        }
    }
}

public struct WeaponAttackCaller : IComponentData
{
    public bool shouldActiveAttack;
    public AttackType currentActiveAttackType;
    public WeaponType currentActiveWeaponType;
    public int currentActiveCombo;

    public bool shouldPassiveAttack;
    public WeaponType currentPassiveWeaponType;
    
    public bool ShouldActiveAttackWithType(WeaponType type, AttackType attackType)
    {
        if (!shouldActiveAttack) return false;

        return type == currentActiveWeaponType && attackType == currentActiveAttackType;
    }
    
    public bool ShouldPassiveAttackWithType(WeaponType type)
    {
        if (!shouldPassiveAttack) return false;

        return type == currentPassiveWeaponType;
    }
}

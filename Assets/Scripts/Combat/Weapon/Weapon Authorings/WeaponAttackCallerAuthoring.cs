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
                        currentAttackType = authoring.currentAttackType,
                        currentWeaponType = authoring.currentWeaponType
                    });
        }
    }
}

public struct WeaponAttackCaller : IComponentData
{
    public bool shouldActiveAttack;
    public AttackType currentAttackType;
    public WeaponType currentWeaponType;

    public bool ShouldActiveAttackWithType(WeaponType type)
    {
        if (!shouldActiveAttack) return false;

        return type == currentWeaponType;
    }
}

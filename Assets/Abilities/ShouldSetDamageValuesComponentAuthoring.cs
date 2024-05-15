using System.Collections;
using System.Collections.Generic;
using Patrik;
using Unity.Entities;
using UnityEngine;

public class ShouldSetDamageValuesComponentAuthoring : MonoBehaviour
{
    public AttackType attackType;
    public WeaponType weaponType;

    public class ShouldSetDamageValuesComponentAuthoringBaker : Baker<ShouldSetDamageValuesComponentAuthoring>
    {
        public override void Bake(ShouldSetDamageValuesComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ShouldSetDamageValuesComponent
                {
                    AttackType = authoring.attackType, WeaponType = authoring.weaponType
                });
        }
    }
}

public struct ShouldSetDamageValuesComponent : IComponentData
{
    public AttackType AttackType;
    public WeaponType WeaponType;
}

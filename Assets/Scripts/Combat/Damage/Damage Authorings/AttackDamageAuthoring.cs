using System.Collections;
using System.Collections.Generic;
using Damage;
using Unity.Entities;
using UnityEngine;


public class AttackDamageAuthoring : MonoBehaviour
{
    [SerializeField] private float attackDamage;
    [SerializeField] private float criticalChance;
    
    class Baker : Baker<AttackDamageAuthoring>
    {
        public override void Bake(AttackDamageAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            DamageContents damageInfo = new DamageContents
            {
                DamageValue = authoring.attackDamage,
                CriticalRate = authoring.criticalChance
            };
            
            AddComponent(entity, new DamageComponent{Value = damageInfo});
            AddComponent(entity, new CachedDamageComponent{Value = damageInfo});
        }
    }
}

/// <summary>
///  Base damage data for a particular entity. Can be updated from upgrades.
/// </summary>
public struct DamageComponent : IComponentData
{
    public DamageContents Value;
}

/// <summary>
/// Cached data that is calculated before, and used for, damage calculations. Should NOT be directly changed from upgrades.
/// </summary>
public struct CachedDamageComponent : IComponentData
{
    public DamageContents Value;
}
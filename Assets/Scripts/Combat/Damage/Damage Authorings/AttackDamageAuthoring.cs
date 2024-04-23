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
            
            AddComponent(entity, new BaseAttackDamageComponent{Value = damageInfo});
            AddComponent(entity, new CurrentAttackDamageComponent{Value = damageInfo});
        }
    }
}

public struct BaseAttackDamageComponent : IComponentData
{
    public DamageContents Value;
}

public struct CurrentAttackDamageComponent : IComponentData
{
    public DamageContents Value;
}

// public struct AttackDamageInfo
// {
//     public float Damage;
//     public float CriticalChance;
// }

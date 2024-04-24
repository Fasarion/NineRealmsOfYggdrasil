using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpecialAttackChargeAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    
    class Baker : Baker<SpecialAttackChargeAuthoring>
    {
        public override void Bake(SpecialAttackChargeAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpecialAttackChargePrefab{Value = GetEntity(authoring.prefab)});
        }
    }
}

public struct SpecialAttackChargePrefab : IComponentData
{
    public Entity Value;
}

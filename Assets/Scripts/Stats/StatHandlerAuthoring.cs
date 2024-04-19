using System.Collections;
using System.Collections.Generic;
using Patrik;
using Unity.Entities;
using UnityEngine;

public class StatHandlerAuthoring : MonoBehaviour
{
    class Baker : Baker<StatHandlerAuthoring>
    {
        public override void Bake(StatHandlerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new StatHandlerComponent()); 
        }
    }
}

public struct StatHandlerComponent : IComponentData
{
    public bool ShouldUpdateStats;
    
    public AttackType AttackType;
    public WeaponType WeaponType;
    public int ComboCounter;
}

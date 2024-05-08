using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using System;

public class ChargeBuffAuthoring : MonoBehaviour 
{
    [SerializeField] List<ChargeStageBuff> chargeStageBuffs;
    
    class Baker : Baker<ChargeBuffAuthoring>
    {
        public override void Bake(ChargeBuffAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            var buffer = AddBuffer<ChargeBuffElement>(entity);

            foreach (var buff in authoring.chargeStageBuffs)
            {
                buffer.Add(new ChargeBuffElement{Value = buff});  
            }
            
            AddComponent(entity, new CachedChargeBuff());
        }
    }
}

[Serializable]
public struct ChargeStageBuff
{
    public float DamageModifier;
    public float RangeModifier;
}

public struct ChargeBuffElement : IBufferElementData
{
    public ChargeStageBuff Value;
}

public struct CachedChargeBuff : IComponentData
{
    public ChargeStageBuff Value;
}
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ChargeAuthoring : MonoBehaviour
{
    [SerializeField] private float chargeSpeed;
    
    class Baker : Baker<ChargeAuthoring>
    {
        public override void Bake(ChargeAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BaseChargeComponent{ChargeSpeed = authoring.chargeSpeed});
            AddComponent(entity, new CachedChargeComponent(){ChargeSpeed = authoring.chargeSpeed});
        }
    }
}

public struct ChargeContents
{
    public float TimeBetweenChargeStages;
    
}



public struct CachedChargeComponent : IComponentData
{
    public float ChargeSpeed;
}

public struct BaseChargeComponent : IComponentData
{
    public float ChargeSpeed;
}

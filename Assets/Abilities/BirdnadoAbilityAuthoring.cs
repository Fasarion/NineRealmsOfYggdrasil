using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
public enum BirdnadoType
{
    None,
    NormalAttack,
    UltimateAttack,
}


public class BirdnadoAbilityAuthoring : MonoBehaviour
{
    public BirdnadoType BirdnadoType;
    
    class Baker : Baker<BirdnadoAbilityAuthoring>
    {
        public override void Bake(BirdnadoAbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BirdnadoComponent());

            if (authoring.BirdnadoType == BirdnadoType.UltimateAttack)
            {
                AddComponent(entity, new PartOfBirdUltimate());
            }
        }
    }
}

public struct BirdnadoComponent : IComponentData{}

public struct PartOfBirdUltimate : IComponentData { }

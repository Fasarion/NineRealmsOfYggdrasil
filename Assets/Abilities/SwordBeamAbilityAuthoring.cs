using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SwordBeamAbilityAuthoring : MonoBehaviour
{
    class Baker : Baker<SwordBeamAbilityAuthoring>
    {
        public override void Bake(SwordBeamAbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new SwordBeamComponent());
        }
    }
}

public struct SwordBeamComponent : IComponentData{}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class IceRingAbilityAuthoring : MonoBehaviour
{
    public class IceRingAbilityAuthoringBaker : Baker<IceRingAbilityAuthoring>
    {
        public override void Bake(IceRingAbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new IceRingAbility());
        }
    }
}

public struct IceRingAbility : IComponentData
{
}

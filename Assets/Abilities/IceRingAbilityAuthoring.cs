using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class IceRingAbilityAuthoring : MonoBehaviour
{
    public bool isInitialized;

    public class IceRingAbilityAuthoringBaker : Baker<IceRingAbilityAuthoring>
    {
        public override void Bake(IceRingAbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new IceRingAbility { isInitialized = authoring.isInitialized });
        }
    }
}

public struct IceRingAbility : IComponentData
{
    public bool isInitialized;
}

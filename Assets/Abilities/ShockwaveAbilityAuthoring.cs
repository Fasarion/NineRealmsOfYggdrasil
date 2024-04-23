using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ShockwaveAbilityAuthoring : MonoBehaviour
{
    public bool hasFired;

    public class ShockwaveAbilityAuthoringBaker : Baker<ShockwaveAbilityAuthoring>
    {
        public override void Bake(ShockwaveAbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShockwaveAbility { HasFired = authoring.hasFired });
        }
    }
}

public struct ShockwaveAbility : IComponentData
{
    public bool HasFired;
}

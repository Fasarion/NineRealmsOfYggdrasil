using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ThunderBoltProjectileAuthoring : MonoBehaviour
{
    public bool hasFired;

    public class ThunderBoltProjectileAuthoringBaker : Baker<ThunderBoltProjectileAuthoring>
    {
        public override void Bake(ThunderBoltProjectileAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ThunderBoltProjectile { HasFired = authoring.hasFired });
        }
    }
}

public struct ThunderBoltProjectile : IComponentData
{
    public bool HasFired;
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HammerSpecialProjectileAuthoring : MonoBehaviour
{
    public bool hasFired;
    public bool isCharging;

    public class HammerSpecialProjectileBaker : Baker<HammerSpecialProjectileAuthoring>
    {
        public override void Bake(HammerSpecialProjectileAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new HammerSpecialProjectile
                {
                    HasFired = authoring.hasFired, IsCharging = authoring.isCharging
                });
        }
    }
}

public struct HammerSpecialProjectile : IComponentData
{
    public bool HasFired;
    public bool IsCharging;
}

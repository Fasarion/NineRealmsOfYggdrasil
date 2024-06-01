using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class HammerSpecialProjectileAuthoring : MonoBehaviour
{
    public bool hasFired;
    public float3 ogPos;
    public HammerSpecialProjectileTransformValues transformValues;
    public float3 directionVector;
    public bool isInitialized;
    public float delayTime;
    public bool isTrailEnabled;

    public class HammerSpecialProjectileAuthoringBaker : Baker<HammerSpecialProjectileAuthoring>
    {
        public override void Bake(HammerSpecialProjectileAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new HammerSpecialProjectile
                {
                    HasFired = authoring.hasFired,
                    OgPos = authoring.ogPos,
                    TransformValues = authoring.transformValues,
                    DirectionVector = authoring.directionVector,
                    IsInitialized = authoring.isInitialized,
                    DelayTime = authoring.delayTime,
                    IsTrailEnabled = authoring.isTrailEnabled
                });
        }
    }
}

public struct HammerSpecialProjectile : IComponentData
{
    public bool HasFired;
    public float3 OgPos;
    public HammerSpecialProjectileTransformValues TransformValues;
    public float3 DirectionVector;
    public bool IsInitialized;
    public float DelayTime;
    public bool IsTrailEnabled;
}

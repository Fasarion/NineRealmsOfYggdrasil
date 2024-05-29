using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class IceRingProjectileAuthoring : MonoBehaviour
{
    [HideInInspector] public bool isInitialized;
    [HideInInspector] public float area;
    [HideInInspector] public bool hasFired;
    [HideInInspector] public int currentAbilityStage;

    public class IceRingProjectileAuthoringBaker : Baker<IceRingProjectileAuthoring>
    {
        public override void Bake(IceRingProjectileAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new IceRingProjectile
                {
                    IsInitialized = authoring.isInitialized,
                    Area = authoring.area,
                    HasFired = authoring.hasFired,
                    CurrentAbilityStage = authoring.currentAbilityStage
                });
        }
    }
}

public struct IceRingProjectile : IComponentData
{
    public bool IsInitialized;
    public float Area;
    public bool HasFired;
    public int CurrentAbilityStage;
}

using System.Collections;
using System.Collections.Generic;
using Patrik;
using Unity.Entities;
using UnityEngine;

public class ShouldSlowPlayerMovementOnAttackComponentAuthoring : MonoBehaviour
{
    [HideInInspector] public float cachedSpeed;
    public float slowPercentage;
    [HideInInspector] public bool isInitialized;
    [HideInInspector] public float slowTimer;
    public float slowDuration;
    public WeaponType weaponType;
    [HideInInspector] public bool isSlowing;

    public class
        ShouldSlowPlayerMovementOnAttackComponentAuthoringBaker : Baker<
        ShouldSlowPlayerMovementOnAttackComponentAuthoring>
    {
        public override void Bake(ShouldSlowPlayerMovementOnAttackComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ShouldSlowPlayerMovementOnAttackComponent
                {
                    CachedSpeed = authoring.cachedSpeed,
                    SlowPercentage = authoring.slowPercentage,
                    IsInitialized = authoring.isInitialized,
                    SlowTimer = authoring.slowTimer,
                    SlowDuration = authoring.slowDuration,
                    WeaponType = authoring.weaponType,
                    IsSlowing = authoring.isSlowing,
                });
        }
    }
}

public struct ShouldSlowPlayerMovementOnAttackComponent : IComponentData
{
    public float CachedSpeed;
    public float SlowPercentage;
    public bool IsInitialized;
    public float SlowTimer;
    public float SlowDuration;
    public WeaponType WeaponType;
    public bool IsSlowing;
}

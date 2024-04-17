using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UltimateAttackCooldownAuthoring : MonoBehaviour
{
    public float currentTime;
    public float maxTime;

    public class UltimateAttackCooldownBaker : Baker<UltimateAttackCooldownAuthoring>
    {
        public override void Bake(UltimateAttackCooldownAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new UltimateAttackCooldown
                    {
                        currentTime = authoring.currentTime, maxTime = authoring.maxTime
                    });
        }
    }
}

public struct UltimateAttackCooldown : IComponentData
{
    public float currentTime;
    public float maxTime;
}

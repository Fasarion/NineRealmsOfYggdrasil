using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpecialAttackCooldownAuthoring : MonoBehaviour
{
    public float currentTime;
    public float maxTime;

    public class SpecialAttackCooldownAuthoringBaker : Baker<SpecialAttackCooldownAuthoring>
    {
        public override void Bake(SpecialAttackCooldownAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new SpecialAttackCooldown
                    {
                        currentTime = authoring.currentTime, maxTime = authoring.maxTime
                    });
        }
    }
}

public struct SpecialAttackCooldown : IComponentData
{
    public float currentTime;
    public float maxTime;
}

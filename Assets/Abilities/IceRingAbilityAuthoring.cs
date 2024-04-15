using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class IceRingAbilityAuthoring : MonoBehaviour
{
    [HideInInspector] public bool isInitialized;
    [HideInInspector] public float area;
    [HideInInspector] public bool hasFired;

    public class IceRingAbilityAuthoringBaker : Baker<IceRingAbilityAuthoring>
    {
        public override void Bake(IceRingAbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new IceRingAbility
                    {
                        isInitialized = authoring.isInitialized, area = authoring.area, hasFired = authoring.hasFired
                    });
        }
    }
}

public struct IceRingAbility : IComponentData
{
    public bool isInitialized;
    public float area;
    public bool hasFired;
}
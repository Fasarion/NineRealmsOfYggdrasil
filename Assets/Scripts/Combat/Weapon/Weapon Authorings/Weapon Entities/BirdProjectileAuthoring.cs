using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BirdProjectileAuthoring : MonoBehaviour
{
    class Baker : Baker<BirdProjectileAuthoring>
    {
        public override void Bake(BirdProjectileAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new BirdProjectileComponent());
            AddComponent(entity, new BirdNormalMovementComponent());
        }
    }
}

public struct BirdProjectileComponent : IComponentData{}

public struct BirdNormalMovementComponent : IComponentData
{
    public float CurrentTValue;

    public float TimeToComplete;

    public float2 startPoint;
    public float2 controlPoint1;
    public float2 controlPoint2;
}

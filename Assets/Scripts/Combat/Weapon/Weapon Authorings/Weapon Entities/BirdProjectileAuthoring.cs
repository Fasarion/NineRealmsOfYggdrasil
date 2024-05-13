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
            
            // bird projectile tag
            AddComponent(entity, new BirdProjectileComponent());
            
            // movement components
            AddComponent(entity, new BirdNormalMovementComponent());
            SetComponentEnabled<BirdNormalMovementComponent>(entity, false);
            
            AddComponent(entity, new BirdSpecialMovementComponent());
            SetComponentEnabled<BirdSpecialMovementComponent>(entity, false);
        }
    }
}

public struct BirdProjectileComponent : IComponentData{}

public struct BirdNormalMovementComponent : IComponentData, IEnableableComponent
{
    public bool HasResetHitBuffer;
    public float CurrentTValue;

    public float TimeToComplete;

    public float2 startPoint;
    public float2 controlPoint1;
    public float2 controlPoint2;
}

public struct BirdSpecialMovementComponent : IComponentData, IEnableableComponent
{
}

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
            AddComponent(entity, new BezierMovementComponent());
            SetComponentEnabled<BezierMovementComponent>(entity, false);
            
            AddComponent(entity, new CircularMovementComponent());
            SetComponentEnabled<CircularMovementComponent>(entity, false);
            
            AddComponent(entity, new DiveMovementComponent());
            SetComponentEnabled<DiveMovementComponent>(entity, false);
        }
    }
}

public struct BirdProjectileComponent : IComponentData{}

public struct BezierMovementComponent : IComponentData, IEnableableComponent
{
    public bool HasResetHitBuffer;
    public float CurrentTValue;

    public float TimeToComplete;

    public float2 startPoint;
    public float2 controlPoint1;
    public float2 controlPoint2;
}

public struct CircularMovementComponent : IComponentData, IEnableableComponent
{
    public float Radius;

    public Entity CenterPointEntity;
    
    public float CurrentAngle;

    public float BaseAngularSpeed;
    public float AngularSpeed;
    
    public bool InUpperHalfCircle;
}

public struct DiveMovementComponent : IComponentData, IEnableableComponent { }

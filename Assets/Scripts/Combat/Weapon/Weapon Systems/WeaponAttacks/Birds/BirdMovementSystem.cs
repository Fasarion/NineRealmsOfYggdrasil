using Damage;
using Movement;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Weapon;

public partial struct BirdMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerPositionSingleton>();
        state.RequireForUpdate<BirdNormalMovementComponent>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value;
        var playerPos2D = playerPos.xz;
        
        // normal attack movement
        foreach (var (birdMovement, moveSpeed, transform, direction, entity) in SystemAPI
            .Query<RefRW<BirdNormalMovementComponent>, MoveSpeedComponent, RefRW<LocalTransform>, RefRW<DirectionComponent>>()
            .WithNone<AutoMoveComponent>()
            .WithEntityAccess())
        {
            // disable this move behaviour when bird has completed its curve
            if (birdMovement.ValueRO.CurrentTValue > 1)
            {
                state.EntityManager.SetComponentEnabled<AutoMoveComponent>(entity, true);
                continue;
            }
            
            // reset hit buffer halfway through motion so bird can damage enemies on the way back
            if (!birdMovement.ValueRO.HasResetHitBuffer && birdMovement.ValueRO.CurrentTValue > 0.5f)
            {
                var hitBuffer = state.EntityManager.GetBuffer<HitBufferElement>(entity);
                hitBuffer.Clear();
                birdMovement.ValueRW.HasResetHitBuffer = true;
            }
            
            birdMovement.ValueRW.CurrentTValue += deltaTime / birdMovement.ValueRO.TimeToComplete;
            
            float2 start = birdMovement.ValueRO.startPoint;
            float2 control1 = birdMovement.ValueRO.controlPoint1;
            float2 control2 = birdMovement.ValueRO.controlPoint2;
            float2 end = playerPos2D;
            
            float2 cubicPos = EvaluateCubicBezier(ref state, start, control1, control2, end, birdMovement.ValueRO.CurrentTValue);
            float2 tangent = EvaluateCubicBezierDerivative(ref state, start, control1, control2, end, birdMovement.ValueRO.CurrentTValue);

            transform.ValueRW.Position = new float3(cubicPos.x, 0, cubicPos.y);
            var directionValue = new float3(tangent.x, 0, tangent.y);
            
            var targetRotation = quaternion.LookRotationSafe(directionValue, math.up());

            // todo: remove magic variable
            float t = 0.5f;
            transform.ValueRW.Rotation = math.slerp(transform.ValueRO.Rotation, targetRotation, t);
            direction.ValueRW.Value = directionValue;
        }

        // special attack movement
        foreach (var (birdMovement, moveSpeed, transform, direction, entity) in SystemAPI
            .Query<RefRW<BirdSpecialMovementComponent>, MoveSpeedComponent, RefRW<LocalTransform>, RefRW<DirectionComponent>>()
            .WithNone<AutoMoveComponent>()
            .WithEntityAccess())
        {
            // move transform in circle
            float angle = birdMovement.ValueRO.CurrentAngle;
            float x = playerPos.x + birdMovement.ValueRO.Radius * math.cos(angle);
            float z = playerPos.z + birdMovement.ValueRO.Radius * math.sin(angle);
            float3 targetPosition = new float3(x, playerPos.y, z);
            transform.ValueRW.Position = targetPosition;
            
            // rotate transform
            quaternion rotation = quaternion.RotateY(-angle);
            transform.ValueRW.Rotation = rotation;
            
            // set new angle
            birdMovement.ValueRW.CurrentAngle += deltaTime * birdMovement.ValueRO.AngularSpeed;
        }
    }
    
    [BurstCompile]
    static float2 EvaluateCubicBezier(ref SystemState state, float2 startPoint, float2 controlPoint1, float2 controlPoint2, float2 endPoint, float t)
    {
        float oneMinusT = 1.0f - t;
        float2 p0 = startPoint;
        float2 p1 = controlPoint1;
        float2 p2 = controlPoint2;
        float2 p3 = endPoint;

        // Cubic Bezier curve equation
        float2 position = math.pow(oneMinusT, 3) * p0 +
                          3 * math.pow(oneMinusT, 2) * t * p1 +
                          3 * oneMinusT * math.pow(t, 2) * p2 +
                          math.pow(t, 3) * p3;

        return position;
    }
    
    [BurstCompile]
    static float2 EvaluateCubicBezierDerivative(ref SystemState state, float2 startPoint, float2 controlPoint1, float2 controlPoint2, float2 endPoint, float t)
    {
        float oneMinusT = 1.0f - t;
        float2 p0 = startPoint;
        float2 p1 = controlPoint1;
        float2 p2 = controlPoint2;
        float2 p3 = endPoint;

        // Derivative of cubic Bezier curve equation
        float2 derivative = 3 * math.pow(oneMinusT, 2) * (p1 - p0) +
                            6 * oneMinusT * t * (p2 - p1) +
                            3 * math.pow(t, 2) * (p3 - p2);

        return derivative;
    }
    
    static float2 EvaluateCubicBezierSecondDerivative(ref SystemState state, float2 startPoint, float2 controlPoint1, float2 controlPoint2, float2 endPoint, float t)
    {
        float oneMinusT = 1.0f - t;
        float2 p0 = startPoint;
        float2 p1 = controlPoint1;
        float2 p2 = controlPoint2;
        float2 p3 = endPoint;

        // Second derivative of cubic Bezier curve equation
        float2 secondDerivative = 6 * (p2 - 2 * p1 + p0) * oneMinusT +
                                  6 * (p3 - 2 * p2 + p1) * t;

        return secondDerivative;
    }
}
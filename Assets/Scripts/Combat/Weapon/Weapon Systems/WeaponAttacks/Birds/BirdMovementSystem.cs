using Damage;
using Destruction;
using Movement;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Weapon;

[UpdateAfter(typeof(UpdateMouseWorldPositionSystem))]
public partial struct BirdMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerPositionSingleton>();        
        state.RequireForUpdate<MousePositionInput>();
        state.RequireForUpdate<GameUnpaused>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value;
        var playerPos2D = playerPos.xz;

        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
        // bezier movement
        foreach (var (bezierMover, transform, direction, entity) in SystemAPI
            .Query<RefRW<BezierMovementComponent>, RefRW<LocalTransform>, RefRW<DirectionComponent>>()
            .WithEntityAccess())
        {
            // disable this move behaviour when bird has completed its curve
            if (bezierMover.ValueRO.CurrentTValue > 1)
            {
               // state.EntityManager.SetComponentEnabled<AutoMoveComponent>(entity, true);
                ecb.AddComponent<ShouldBeDestroyed>(entity);
                continue;
            }
            
            // reset hit buffer halfway through motion so bird can damage enemies on the way back
            if (!bezierMover.ValueRO.HasResetHitBuffer && bezierMover.ValueRO.CurrentTValue > 0.5f)
            {
                var hitBuffer = state.EntityManager.GetBuffer<HitBufferElement>(entity);
                hitBuffer.Clear();
                bezierMover.ValueRW.HasResetHitBuffer = true;
            }
            
            bezierMover.ValueRW.CurrentTValue += deltaTime / bezierMover.ValueRO.TimeToComplete;
            
            float2 start = bezierMover.ValueRO.startPoint;
            float2 control1 = bezierMover.ValueRO.controlPoint1;
            float2 control2 = bezierMover.ValueRO.controlPoint2;
            float2 end = playerPos2D;

            float currentTValue = bezierMover.ValueRO.CurrentTValue; 
            
            float2 cubicPos = EvaluateCubicBezier(ref state, ref start, ref control1, ref control2, ref end, ref currentTValue);
            float2 tangent = EvaluateCubicBezierDerivative(ref state, ref start, ref control1, ref control2, ref end, ref currentTValue);

            transform.ValueRW.Position = new float3(cubicPos.x, 0, cubicPos.y);
            var directionValue = new float3(tangent.x, 0, tangent.y);
            
            var targetRotation = quaternion.LookRotationSafe(directionValue, math.up());

            // todo: remove magic variable
            float t = 0.5f;
            transform.ValueRW.Rotation = math.slerp(transform.ValueRO.Rotation, targetRotation, t);
            direction.ValueRW.Value = directionValue;
        }

        // circular movement
        foreach (var (circleMover, moveSpeed, transform, direction, entity) in SystemAPI
            .Query<RefRW<CircularMovementComponent>, MoveSpeedComponent, RefRW<LocalTransform>, RefRW<DirectionComponent>>()
            .WithEntityAccess())
        {
            float angleLastFrame = circleMover.ValueRO.CurrentAngle;
            
            // move transform in circle
            float angle = circleMover.ValueRO.CurrentAngle;
            float sinY = math.sin(angle);
            float cosY = math.cos(angle);

            float3 targetPosition = new float3
            {
                x = circleMover.ValueRO.Radius * cosY,
                y = 0,
                z = circleMover.ValueRO.Radius * sinY,
            };

            if (circleMover.ValueRO.CenterPointEntity != Entity.Null)
            {
                var centerPointTransform = SystemAPI.GetComponent<LocalTransform>(circleMover.ValueRO.CenterPointEntity);
                targetPosition += centerPointTransform.Position;
            }

            transform.ValueRW.Position = targetPosition;
            
            // rotate transform
            quaternion rotation = quaternion.RotateY(-angle);
            transform.ValueRW.Rotation = rotation;
            
            // set new angle
            var nextAngle = angleLastFrame + deltaTime * circleMover.ValueRO.AngularSpeed;
            circleMover.ValueRW.CurrentAngle = nextAngle;

            bool wasInUpperCircle = circleMover.ValueRO.InUpperHalfCircle;
            bool nowInUpperCircle = sinY >= 0;
            
            // Reset hit buffer after every half lap
            if (nowInUpperCircle != wasInUpperCircle)
            {
                var hitBuffer = state.EntityManager.GetBuffer<HitBufferElement>(entity);
                hitBuffer.Clear();
                circleMover.ValueRW.InUpperHalfCircle = !wasInUpperCircle;
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
    
    [BurstCompile]
    float2 EvaluateCubicBezier(ref SystemState state, ref float2 startPoint, ref float2 controlPoint1, ref float2 controlPoint2, ref float2 endPoint, ref float t)
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
    float2 EvaluateCubicBezierDerivative(ref SystemState state, ref float2 startPoint, ref float2 controlPoint1, ref float2 controlPoint2, ref float2 endPoint, ref float t)
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
    
    float2 EvaluateCubicBezierSecondDerivative(ref SystemState state, float2 startPoint, float2 controlPoint1, float2 controlPoint2, float2 endPoint, float t)
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
using System.Collections;
using System.Collections.Generic;
using Destruction;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[BurstCompile]
public partial struct HammerSpecialProjectileSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<HammerSpecialConfig>();
        state.RequireForUpdate<HammerComponent>();
        state.RequireForUpdate<PlayerTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingleton<WeaponAttackCaller>();
        var config = SystemAPI.GetSingleton<HammerSpecialConfig>();
        ChargeState currentChargeState = attackCaller.SpecialChargeInfo.chargeState;
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var hammerEntity = SystemAPI.GetSingletonEntity<HammerComponent>();
        var hammerTransform = state.EntityManager.GetComponentData<LocalTransform>(hammerEntity);
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerTransform = state.EntityManager.GetComponentData<LocalTransform>(playerEntity);

        float currentDelayTime = config.TimeBetweenInitialThrowAndProjectiles;

        foreach (var (projectile, transform, timer, entity) in
                 SystemAPI.Query<RefRW<HammerSpecialProjectile>,  RefRW<LocalTransform>, RefRW<TimerObject>>()
                     .WithEntityAccess())
        {
            if (!projectile.ValueRO.IsInitialized)
            {
                ecb.SetComponentEnabled<GameObjectParticlePrefab>(entity, false);
                projectile.ValueRW.IsInitialized = true;
                

            }
            
            if (currentChargeState == ChargeState.Stop && !projectile.ValueRO.HasFired)
            {
                projectile.ValueRW.HasFired = true;
                projectile.ValueRW.OgPos = transform.ValueRO.Position;
                var direction = playerTransform.Forward();
                direction.y = 0;
                projectile.ValueRW.DirectionVector = direction;
                currentDelayTime += config.TimeBetweenProjectileFires;
                projectile.ValueRW.DelayTime = currentDelayTime;
            }

            if (projectile.ValueRO.HasFired)
            {
                if (timer.ValueRO.currentTime < projectile.ValueRO.DelayTime)
                {
                    timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;
                    continue;
                }

                if (!projectile.ValueRO.IsTrailEnabled)
                {
                    projectile.ValueRW.IsTrailEnabled = true;
                    ecb.SetComponentEnabled<GameObjectParticlePrefab>(entity, true);
                    var audioBuffer = SystemAPI.GetSingletonBuffer<AudioBufferData>();
                    audioBuffer.Add(new AudioBufferData { AudioData = config.throwingAudioData});
                }
                
                if (math.length(transform.ValueRO.Position - projectile.ValueRO.OgPos) > config.DistanceToTravel)
                {
                    ecb.AddComponent<ShouldBeDestroyed>(entity);
                }
                
                transform.ValueRW.Position += projectile.ValueRO.DirectionVector * config.TravelForwardSpeed * SystemAPI.Time.DeltaTime;
            }
            else
            {
                var newTransform = GetTransformValues(projectile.ValueRO.TransformValues.hOffset,
                    projectile.ValueRO.TransformValues.wOffset,
                    projectile.ValueRO.TransformValues.isOffsetPositive, hammerTransform, playerTransform);
            
                ecb.SetComponent(entity, newTransform);
            }
            
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
    
    [BurstCompile]
    private LocalTransform GetTransformValues(float randomHOffset, float randomWOffset, bool isDirectionPositive, LocalTransform hammerTransform,
         LocalTransform playerTransform)
    {
        var hOffset = playerTransform.Forward() * (randomHOffset);
        //var hOffset = playerTransform.Up() * (randomHOffset - hammerTransform.Position.y);
        var wOffset = playerTransform.Forward() * randomWOffset;
        
        quaternion rotationQ;

        if (isDirectionPositive)
        {
            rotationQ = quaternion.RotateY(math.radians(90));
            wOffset = math.rotate(rotationQ, wOffset);
        }
        else
        {
            rotationQ = quaternion.RotateY(math.radians(-90));
            wOffset = math.rotate(rotationQ, wOffset);
        }

        LocalTransform transform = new LocalTransform
        {
            Position = playerTransform.Position + wOffset + hOffset,
            Rotation = hammerTransform.Rotation,
            Scale = hammerTransform.Scale,
        };

        return transform;
    }
}

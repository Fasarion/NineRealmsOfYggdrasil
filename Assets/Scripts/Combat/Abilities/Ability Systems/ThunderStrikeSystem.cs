using System.Collections;
using System.Collections.Generic;
using AI;
using Damage;
using Destruction;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

[UpdateBefore(typeof(HitStopSystem))]
[UpdateBefore(typeof(HandleHitBufferSystem))]
[BurstCompile]
public partial struct ThunderStrikeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTargetInfoSingleton>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<ThunderStrikeAbility>();
        state.RequireForUpdate<ThunderStrikeConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var thunderConfig = SystemAPI.GetSingleton<ThunderStrikeConfig>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var target = SystemAPI.GetSingleton<PlayerTargetInfoSingleton>();
        var targetPos = target.LastPosition;

        foreach (var (ability, timer, originTransform, entity) in
                 SystemAPI.Query<RefRW<ThunderStrikeAbility>, RefRW<TimerObject>, RefRW<LocalTransform>>()
                     .WithEntityAccess())
        {
            
            if (ability.ValueRO.strikeCounter >= thunderConfig.maxStrikes)
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);
            }
            
            
            if (!ability.ValueRO.isInitialized)
            {
                ability.ValueRW.isInitialized = true;

                originTransform.ValueRW.Position = targetPos + new float3(0, thunderConfig.mainEffectHeightOffset, 0);
                originTransform.ValueRW.Rotation = Quaternion.Euler(0f, 0f, 0f);
                originTransform.ValueRW.Scale = 1;
            }

            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;


            float currentCheckpointTime = thunderConfig.timeBetweenStrikes;
            
            if (!ability.ValueRO.hasDoneFirstStrike)
            {
                currentCheckpointTime = thunderConfig.initialStrikeDelay;
                ability.ValueRW.hasDoneFirstStrike = true;
            }
            
            if (timer.ValueRO.currentTime > currentCheckpointTime)
            {
                ability.ValueRW.strikeCounter++;
                
                var effect = state.EntityManager.Instantiate(thunderConfig.projectileAbilityPrefab);
                
                state.EntityManager.SetComponentData(effect, new LocalTransform
                {
                    Position = targetPos + new float3(0, thunderConfig.shockwaveEffectHeightOffset, 0),
                    Rotation = Quaternion.Euler(-90f, 0f, 0f),
                    Scale = 1
                });

                timer.ValueRW.currentTime = 0;
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

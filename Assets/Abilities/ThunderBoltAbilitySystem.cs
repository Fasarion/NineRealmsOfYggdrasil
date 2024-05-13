using System.Collections;
using System.Collections.Generic;
using Damage;
using Destruction;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(CombatSystemGroup))]
[UpdateBefore(typeof(AddDamageBufferElementOnTriggerSystem))]
public partial struct ThunderBoltAbilitySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerPositionSingleton>();
        state.RequireForUpdate<ThunderBoltConfig>();
        state.RequireForUpdate<PlayerRotationSingleton>();
        state.RequireForUpdate<ThunderBoltAbility>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerRotation = SystemAPI.GetSingleton<PlayerRotationSingleton>();
        var playerPosition = SystemAPI.GetSingleton<PlayerPositionSingleton>();
        var config = SystemAPI.GetSingleton<ThunderBoltConfig>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        foreach (var (ability, timer, entity) in
                 SystemAPI.Query<RefRW<ThunderBoltAbility>, RefRW<TimerObject>>()
                     .WithEntityAccess())
        {
            if (ability.ValueRO.CurrentCount >= config.MaxStrikes)
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);
            }

            if (!ability.ValueRO.isInitialized)
            {
                var rotation = playerRotation.Value;
                var directionVector = math.forward(rotation);
                var targetBuffer = state.EntityManager.GetBuffer<TargetBufferElement>(entity);

                for (int i = 0; i < config.MaxStrikes; i++)
                {
                    float3 pos = playerPosition.Value
                                 + directionVector * (config.StrikeSpacing * (i + 1));
                    var element = new TargetBufferElement
                    {
                        Position = pos,
                    };
                    targetBuffer.Add(element);
                }

                ability.ValueRW.isInitialized = true;
            }
            
            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;

            float timerCheckpoint = ability.ValueRO.CurrentCount * config.TimeBetweenStrikes;

            if (timerCheckpoint < timer.ValueRO.currentTime)
            {
                var projectile = state.EntityManager.Instantiate(config.ProjectilePrefab);
                var targetBuffer = state.EntityManager.GetBuffer<TargetBufferElement>(entity);
                var pos = targetBuffer[ability.ValueRO.CurrentCount].Position;

                state.EntityManager.SetComponentData(projectile, new LocalTransform
                {
                    Position = pos
                               + new float3(0, config.VfxHeightOffset, 0),
                    Rotation = quaternion.identity,
                    Scale = 1,
                });

                ability.ValueRW.CurrentCount++;
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

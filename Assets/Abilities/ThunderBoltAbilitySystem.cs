using System.Collections;
using System.Collections.Generic;
using Damage;
using Destruction;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(CombatSystemGroup))]
[UpdateBefore(typeof(AddDamageBufferElementOnTriggerSystem))]
[UpdateBefore(typeof(AddKnockBackOnHitSystem))]
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
        var config = SystemAPI.GetSingletonRW<ThunderBoltConfig>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var configEntity = SystemAPI.GetSingletonEntity<ThunderBoltConfig>();

        foreach (var (ability, timer, entity) in
                 SystemAPI.Query<RefRW<ThunderBoltAbility>, RefRW<TimerObject>>()
                     .WithEntityAccess())
        {
            if (ability.ValueRO.CurrentCount >= config.ValueRW.MaxStrikes)
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);
            }

            if (!ability.ValueRO.isInitialized)
            {
                var spawnCount = state.EntityManager.GetComponentData<SpawnCount>(configEntity);
                config.ValueRW.MaxStrikes = spawnCount.Value;

                var spawnMultiplier = state.EntityManager.GetComponentData<SpawnCountMultiplier>(configEntity);
                config.ValueRW.MaxRows = spawnMultiplier.Value;
                
                var targetBuffer = state.EntityManager.GetBuffer<TargetBufferElement>(entity);

                for (int j = 0; j < config.ValueRW.MaxRows; j++)
                {
                    var rotation = playerRotation.Value;
                    var directionVector = math.forward(rotation);
                    
                    if (j != 0)
                    {
                        float angle;
                        quaternion rotationQ;
                    
                        if (j % 2 == 0)
                        {
                            angle = config.ValueRO.RowsAngle * (j) / 2;
                            rotationQ = quaternion.RotateY(math.radians(angle));
                        }
                        else
                        {
                            angle = -(config.ValueRO.RowsAngle * (j + 1) / 2);
                            rotationQ = quaternion.RotateY(math.radians(angle));
                        }
                    
                        directionVector = math.rotate(rotationQ, directionVector);
                    }
                    
                    for (int i = 0; i < config.ValueRO.MaxStrikes; i++)
                    {
                        
                        float3 pos = playerPosition.Value
                                     + directionVector * (config.ValueRO.StrikeSpacing * (i + 1));
                        var element = new TargetBufferElement
                        {
                            Position = pos,
                        };
                        targetBuffer.Add(element);
                    }
                }
                ability.ValueRW.isInitialized = true;
            }
            
            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;

            float timerCheckpoint = ability.ValueRO.CurrentCount * config.ValueRO.TimeBetweenStrikes;

            if (timerCheckpoint < timer.ValueRO.currentTime)
            {
                for (int i = 0; i < config.ValueRO.MaxRows; i++)
                {
                    var projectile = state.EntityManager.Instantiate(config.ValueRO.ProjectilePrefab);
                    var targetBuffer = state.EntityManager.GetBuffer<TargetBufferElement>(entity);

                    int index = ability.ValueRO.CurrentCount + (config.ValueRW.MaxStrikes * i);
                    if (index >= targetBuffer.Length)
                    {
                        Debug.LogError("Index out range for thunder bolt target buffer.");
                        continue;
                    }
                    
                    var pos = targetBuffer[index].Position;

                    state.EntityManager.SetComponentData(projectile, new LocalTransform
                    {
                        Position = pos
                                   + new float3(0, config.ValueRO.VfxHeightOffset, 0),
                        Rotation = quaternion.identity,
                        Scale = 1,
                    });
                    state.EntityManager.SetComponentData(projectile, new UpdateStatsComponent
                    {
                        EntityToTransferStatsFrom = configEntity,
                    });


                
                    // Handle  audio
                    var audioBuffer = SystemAPI.GetSingletonBuffer<AudioBufferData>();
                    audioBuffer.Add(new AudioBufferData { AudioData = config.ValueRO.HitAudio}); 
                }
                
                ability.ValueRW.CurrentCount++;
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

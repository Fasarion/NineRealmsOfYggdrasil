using System.Collections;
using System.Collections.Generic;
using Destruction;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct SwordProjectileAbilitySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SwordComponent>();
        state.RequireForUpdate<SwordComboAbilityConfig>();
        state.RequireForUpdate<GameUnpaused>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
        var config = SystemAPI.GetSingletonRW<SwordComboAbilityConfig>();

        foreach (var (ability, timer, entity) in SystemAPI
                     .Query<RefRW<SwordProjectileAbility>, RefRW<TimerObject>>()
                     .WithEntityAccess())
        {
            if (!ability.ValueRO.IsInitialized)
            {
                var buffer = SystemAPI.GetSingletonBuffer<SwordTrajectoryRecordingElement>();
                ability.ValueRW.IsInitialized = true;
                ability.ValueRW.bufferLength = buffer.Length - 1;
                var configEntity = SystemAPI.GetSingletonEntity<SwordComboAbilityConfig>();
                var spawnCount = state.EntityManager.GetComponentData<SpawnCount>(configEntity);
                config.ValueRW.SwordCount = spawnCount.Value;
            }
            
            if (ability.ValueRO.CurrentSpawnCount >= config.ValueRO.SwordCount)
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);
                continue;
            }

            float maxTime = config.ValueRO.DelayBetweenSwords;

            if (ability.ValueRO.CurrentSpawnCount == 0)
            {
                maxTime = config.ValueRO.InitialSpawnDelay;
            }

            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;

            if (timer.ValueRO.currentTime > maxTime)
            {
                var projectile = state.EntityManager.Instantiate(config.ValueRO.SwordProjectilePrefab);
                //TODO: add spawn effect
                state.EntityManager.SetComponentData(projectile, new SwordProjectile
                {
                    BufferLength = ability.ValueRO.bufferLength,
                });

                var swordEntity = SystemAPI.GetSingletonEntity<SwordComponent>();
                
                state.EntityManager.SetComponentData(projectile, new HasOwnerWeapon
                {
                    OwnerEntity = swordEntity,
                    OwnerWasActive = true,
                });
                
                state.EntityManager.SetComponentData(projectile, new UpdateStatsComponent
                {
                    EntityToTransferStatsFrom = swordEntity,
                });

                ability.ValueRW.CurrentSpawnCount++;
                timer.ValueRW.currentTime = 0;
            }
        }
    }
}

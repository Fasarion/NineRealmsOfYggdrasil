using System.Collections;
using System.Collections.Generic;
using Destruction;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct HammerSpecialProjectileAbilitySystem : ISystem
{
    private bool _lastSpawnSideWasLeft;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<HammerComponent>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<HammerSpecialConfig>();
        state.RequireForUpdate<HammerSpecialAbility>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingleton<WeaponAttackCaller>();
        var config = SystemAPI.GetSingletonRW<HammerSpecialConfig>();
        ChargeState currentChargeState = attackCaller.SpecialChargeInfo.chargeState;
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        RefRW<RandomComponent> random = SystemAPI.GetSingletonRW<RandomComponent>();
        var hammerEntity = SystemAPI.GetSingletonEntity<HammerComponent>();
        var configEntity = SystemAPI.GetSingletonEntity<HammerSpecialConfig>();
        
        if (attackCaller.ActiveAttackData.WeaponType != WeaponType.Hammer) return;

        var spawnCount = state.EntityManager.GetComponentData<SpawnCount>(configEntity);
        config.ValueRW.MaxProjectiles = spawnCount.Value;

        foreach (var (ability, timer, entity) in
                 SystemAPI.Query<RefRW<HammerSpecialAbility>, RefRW<TimerObject>>()
                     .WithEntityAccess())
        {
            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;
            
            if (timer.ValueRO.currentTime > config.ValueRO.TimeBetweenSpawns && !ability.ValueRO.HasFired && ability.ValueRO.CurrentSpawnCount < config.ValueRO.MaxProjectiles)
            {
                timer.ValueRW.currentTime = 0;
                ability.ValueRW.CurrentSpawnCount++;
                var projectile = state.EntityManager.Instantiate(config.ValueRO.HammerProjectilePrefab);
                var vfx = state.EntityManager.Instantiate(config.ValueRO. HammerSparkPrefab);
                state.EntityManager.SetComponentData(vfx, new VisualEffectComponent
                {
                    EntityToFollow = projectile,
                    ActiveTime = 1f,
                    ShouldFollowParentEntity = true,
                });
                HammerSpecialProjectileTransformValues values =
                    GetTransformValues(random, config, ability.ValueRO.CurrentSpawnCount);
                
                var weapon = state.EntityManager.GetComponentData<WeaponComponent>(hammerEntity);
                
                state.EntityManager.SetComponentData(projectile, new HasOwnerWeapon
                {
                    OwnerEntity = hammerEntity,
                    OwnerWasActive = weapon.InActiveState
                });
                
                state.EntityManager.SetComponentData(projectile, new UpdateStatsComponent
                {
                    EntityToTransferStatsFrom = hammerEntity,
                });

                ecb.SetComponent(projectile, new HammerSpecialProjectile
                {
                    HasFired = false,
                    TransformValues = values,
                });
            }
            
            if (currentChargeState == ChargeState.Stop)
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    private HammerSpecialProjectileTransformValues GetTransformValues(RefRW<RandomComponent> random,
        RefRW<HammerSpecialConfig> config, int currentSpawnCount)
    {
        var randomHOffset = random.ValueRW.random.NextFloat(config.ValueRO. MinSpawnHeight, config.ValueRO.MaxSpawnHeight);
        var randomWOffset = random.ValueRW.random.NextFloat(config.ValueRO.MinSpawnWidth, config.ValueRO.MaxSpawnWidth) +
                            config.ValueRO.SpawnWidthGrowth * currentSpawnCount;

        bool widthDirectionIsPositive;
        
        if (_lastSpawnSideWasLeft)
        {
            widthDirectionIsPositive = true;
            _lastSpawnSideWasLeft = false;
        }
        else
        {
            widthDirectionIsPositive = false;
            _lastSpawnSideWasLeft = true;
        }

        var result = new HammerSpecialProjectileTransformValues
        {
            wOffset = randomWOffset,
            hOffset = randomHOffset,
            isOffsetPositive = widthDirectionIsPositive,
        };

        return result;
    }
}

public struct HammerSpecialProjectileTransformValues
{
    public float wOffset;
    public float hOffset;
    public bool isOffsetPositive;
}

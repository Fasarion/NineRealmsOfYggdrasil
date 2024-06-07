using System.Collections;
using System.Collections.Generic;
using Damage;
using Destruction;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

//[UpdateInGroup(typeof(CombatSystemGroup))]
//[UpdateBefore(typeof(HitStopSystem))]
 [UpdateAfter(typeof(HandleHitBufferSystem))]
// [UpdateBefore(typeof(FillEnergyOnHitSystem))]
[BurstCompile]
public partial struct HammerPassiveAbilitySystem : ISystem
{
    private CollisionFilter _detectionFilter;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<ThunderBoltConfig>();
        state.RequireForUpdate<PlayerPositionSingleton>();
        state.RequireForUpdate<HammerPassiveAbilityConfig>();
        state.RequireForUpdate<HammerPassiveAbility>();
        _detectionFilter = new CollisionFilter
        {
            BelongsTo = 1, // Projectile
            CollidesWith = 1 << 1 // Enemy
        };
    }

    public void OnUpdate(ref SystemState state)
    {   var config = SystemAPI.GetSingleton<HammerPassiveAbilityConfig>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>();
        //var targetPositions = new NativeArray<float3>(config.StrikeCount, Allocator.Persistent);
        var boltConfig = SystemAPI.GetSingleton<ThunderBoltConfig>();
        var passiveEntity = SystemAPI.GetSingletonEntity<HammerPassiveAbilityConfig>();

        foreach (var (ability, timer, entity) in
                 SystemAPI.Query<RefRW<HammerPassiveAbility>, RefRW<TimerObject>>()
                     .WithEntityAccess())
        {
            if (ability.ValueRO.CurrentStrikeCheckpoint >= config.StrikeCount)
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);
                continue;
            }

            if (!ability.ValueRO.HasFired)
            {

                timer.ValueRW.currentTime = 0;
                
                var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
                var hits = new NativeList<DistanceHit>(state.WorldUpdateAllocator);

                float totalArea;
                if(ability.ValueRW.CurrentStrikeCheckpoint == 0) totalArea = config.InitialRadius;
                else totalArea = config.ChainRadius;
                 
                float3 originPosition;

                if (ability.ValueRO.CurrentStrikeCheckpoint == 0) originPosition = playerPos.Value;
                else originPosition = ability.ValueRO.OgStrikePosition;

                hits.Clear();
                
                if (collisionWorld.OverlapSphere(originPosition, totalArea,
                        ref hits, _detectionFilter))
                {
                    foreach (var hit in hits)
                    {
                        if (state.EntityManager.HasComponent<HasBeenThunderStruckComponent>(hit.Entity)) continue;
                        
                        ecb.AddComponent<HasBeenThunderStruckComponent>(hit.Entity);
                        
                        if (ability.ValueRO.CurrentStrikeCheckpoint == 0)
                        {
                            ability.ValueRW.OgStrikePosition = hit.Position;
                        }

                        var bolt = state.EntityManager.Instantiate(boltConfig.ProjectilePrefab);
                        state.EntityManager.SetComponentData(bolt, new LocalTransform
                        {
                            Position = hit.Position
                                       + new float3(0, config.VFXHeightOffset, 0),
                            Rotation = quaternion.identity,
                            Scale = 1,
                        });
                        state.EntityManager.SetComponentData(bolt, new UpdateStatsComponent
                        {
                            EntityToTransferStatsFrom = passiveEntity,
                        });
                        
                        // handle energy
                        // get owner - hammer component
                        var ownerEntity = SystemAPI.GetSingletonEntity<HammerComponent>();
                        
                        state.EntityManager.SetComponentData(bolt, new HasOwnerWeapon
                        {
                            OwnerEntity = ownerEntity,
                            OwnerWasActive = false
                        });

                        
        
                        // // set owner
                        // ecb.AddComponent<HasOwnerWeapon>(bolt);
                        // HasOwnerWeapon hasOwnerWeapon = new HasOwnerWeapon()
                        // {
                        //     OwnerEntity = ownerEntity,
                        //     OwnerWasActive = false
                        // };
                        //
                        // ecb.SetComponent(bolt, hasOwnerWeapon);
                        //
                        // // set energy fill
                        // ecb.AddComponent<EnergyFillComponent>(bolt);
                        // EnergyFillComponent hammerFill =
                        //     state.EntityManager.GetComponentData<EnergyFillComponent>(ownerEntity);
                        //
                        // ecb.SetComponent(bolt, hammerFill);

                        break;
                    }
                }
                
                // Handle  audio
                var audioBuffer = SystemAPI.GetSingletonBuffer<AudioBufferData>();
                audioBuffer.Add(new AudioBufferData { AudioData = config.HitAudio});

                ability.ValueRW.CurrentStrikeCheckpoint++;
                ability.ValueRW.HasFired = true;
            }

            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;

            if (timer.ValueRO.currentTime > config.TimeBetweenStrikes)
            {
                ability.ValueRW.HasFired = false;
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

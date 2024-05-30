using System.Collections;
using System.Collections.Generic;
using Destruction;
using Movement;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.VFX;

[BurstCompile]
//[UpdateAfter(typeof(HandleAnimationSystem))]
[UpdateAfter(typeof(AttackStatTransferSystem))]

public partial struct SwordUltimateAttackSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<SwordComponent>();
        state.RequireForUpdate<SwordUltimateConfig>();
        state.RequireForUpdate<AudioBufferData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        var ultConfig = SystemAPI.GetSingletonRW<SwordUltimateConfig>();
        var swordEntity = SystemAPI.GetSingletonEntity<SwordComponent>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        if (ultConfig.ValueRO.IsActive)
        {
            ultConfig.ValueRW.CurrentTime += Time.deltaTime;

            if (ultConfig.ValueRW.CurrentTime > ultConfig.ValueRO.BeamSpawnTimeAfterAttackStart)
            {
                SpawnSwordBeam(ref state, ultConfig, ecb);
                ultConfig.ValueRW.CurrentTime = -1000f; // hard reset timer
            }
            
            bool stoppedSwordAttack = attackCaller.ValueRO.ActiveAttackData.ShouldStopAttack(WeaponType.Sword) ||
                                      attackCaller.ValueRO.PassiveAttackData.ShouldStopAttack(WeaponType.Sword);
            if (stoppedSwordAttack)
            {
                ultConfig.ValueRW.CurrentAttackCount++;
                
                if (ultConfig.ValueRO.CurrentAttackCount > ultConfig.ValueRO.NumberOfScaledAttacks)
                {
                    var scaleComp = state.EntityManager.GetComponentData<SizeComponent>(swordEntity);
                    scaleComp.Value -= ultConfig.ValueRO.ScaleIncrease;
                    state.EntityManager.SetComponentData(swordEntity, scaleComp);

                    var statHandler = SystemAPI.GetSingletonRW<StatHandlerComponent>();
                    statHandler.ValueRW.ShouldUpdateStats = true;
                
                    ultConfig.ValueRW.IsActive = false;
                }
            }
        }
        
        if (!attackCaller.ValueRO.ShouldStartActiveAttack(WeaponType.Sword, AttackType.Ultimate))
            return;

        ultConfig.ValueRW.CurrentTime = 0f;

        // Initialize attack
        if (!ultConfig.ValueRO.IsActive)
        {
            var scaleComp = state.EntityManager.GetComponentData<SizeComponent>(swordEntity);

            float newSize = scaleComp.Value += ultConfig.ValueRO.ScaleIncrease;
            
            scaleComp.Value = newSize;
            state.EntityManager.SetComponentData(swordEntity, scaleComp);
            
            var statHandler = SystemAPI.GetSingletonRW<StatHandlerComponent>();
            statHandler.ValueRW.ShouldUpdateStats = true;

            ultConfig.ValueRW.IsActive = true;
            ultConfig.ValueRW.CurrentAttackCount = 0;

            //SpawnSwordBeam(ref state, ultConfig, ecb);
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private void SpawnSwordBeam(ref SystemState state, RefRW<SwordUltimateConfig> ultConfig, EntityCommandBuffer ecb)
    {
        // spawn sword beams
        var beamEntity = state.EntityManager.Instantiate(ultConfig.ValueRO.BeamEntityPrefab);

        var playerRot = SystemAPI.GetSingleton<PlayerRotationSingleton>();

        LocalTransform beamTransform = new LocalTransform
        {
            Position = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value,
            Rotation = playerRot.Value,
            Scale = 1
        };
        state.EntityManager.SetComponentData(beamEntity, beamTransform);
        state.EntityManager.SetComponentData(beamEntity, new DirectionComponent {Value = playerRot.Forward});

        var beamVfx = state.EntityManager.Instantiate(ultConfig.ValueRO.BeamVfxPrefab);
        state.EntityManager.SetComponentData(beamVfx, beamTransform);


        var configEntity = SystemAPI.GetSingletonEntity<SwordUltimateConfig>();

        // update stats
        ecb.AddComponent<UpdateStatsComponent>(beamEntity);
        UpdateStatsComponent updateStatsComponent = new UpdateStatsComponent
            {EntityToTransferStatsFrom = configEntity};
        ecb.SetComponent(beamEntity, updateStatsComponent);
    }
}

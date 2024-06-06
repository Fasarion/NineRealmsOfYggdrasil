using Movement;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(AttackStatTransferSystem))]
[UpdateAfter(typeof(PlayerRotationSystem))]
[UpdateAfter(typeof(CombatStatHandleSystem))]
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
        var audioBuffer = SystemAPI.GetSingletonBuffer<AudioBufferData>();

        if (ultConfig.ValueRW.PrepareBeam)
        {
            ultConfig.ValueRW.CurrentTime += Time.deltaTime;
            
            if (ultConfig.ValueRW.CurrentTime > ultConfig.ValueRO.BeamSpawnTimeAfterAttackStart)
            {
                SpawnSwordBeam(ref state, ultConfig, ecb);
                ultConfig.ValueRW.CurrentTime = -1000f; // hard reset timer
                ultConfig.ValueRW.PrepareBeam = false;
                
                //audio
                var audioElement = new AudioBufferData() { AudioData = ultConfig.ValueRO.swingAudioData };
                audioBuffer.Add(audioElement);
            }
        }
        

        if (ultConfig.ValueRO.IsActive)
        {
            // ultConfig.ValueRW.CurrentTime += Time.deltaTime;
            //
            // if (ultConfig.ValueRW.CurrentTime > ultConfig.ValueRO.BeamSpawnTimeAfterAttackStart)
            // {
            //     SpawnSwordBeam(ref state, ultConfig, ecb);
            //     ultConfig.ValueRW.CurrentTime = -1000f; // hard reset timer
            // }
            
            bool stoppedSwordAttack = attackCaller.ValueRO.ActiveAttackData.ShouldStopAttack(WeaponType.Sword) ||
                                      attackCaller.ValueRO.PassiveAttackData.ShouldStopAttack(WeaponType.Sword);
            if (stoppedSwordAttack)
            {
                ultConfig.ValueRW.CurrentAttackCount++;

                var ultConfigEntity = SystemAPI.GetSingletonEntity<SwordUltimateConfig>();
                var spawnCount = state.EntityManager.GetComponentData<SpawnCount>(ultConfigEntity);
                var spawnMultiplier = state.EntityManager.GetComponentData<SpawnCountMultiplier>(ultConfigEntity);

                ultConfig.ValueRW.NumberOfScaledAttacks = spawnMultiplier.Value;
                ultConfig.ValueRW.BeamsPerSwing = spawnCount.Value;
                
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

        if (attackCaller.ValueRO.ShouldStartActiveAttack(WeaponType.Sword, AttackType.Ultimate))
        {
            ultConfig.ValueRW.CurrentTime = 0f;
            ultConfig.ValueRW.PrepareBeam = true;

           
            // Initialize attack
            if (!ultConfig.ValueRO.IsActive)
            {
                var audioElement = new AudioBufferData() { AudioData = ultConfig.ValueRO.onUseAudioData };
                audioBuffer.Add(audioElement);
                var scaleComp = state.EntityManager.GetComponentData<SizeComponent>(swordEntity);

                float newSize = scaleComp.Value += ultConfig.ValueRO.ScaleIncrease;
            
                scaleComp.Value = newSize;
                state.EntityManager.SetComponentData(swordEntity, scaleComp);
            
                var statHandler = SystemAPI.GetSingletonRW<StatHandlerComponent>();
                statHandler.ValueRW.ShouldUpdateStats = true;

                ultConfig.ValueRW.IsActive = true;
                ultConfig.ValueRW.CurrentAttackCount = 0;
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private void SpawnSwordBeam(ref SystemState state, RefRW<SwordUltimateConfig> ultConfig, EntityCommandBuffer ecb)
    {
        int beamCount = ultConfig.ValueRO.BeamsPerSwing;
        
        if (beamCount <= 0) return;

        for (int i = 0; i < beamCount; i++)
        {
            var playerRot = SystemAPI.GetSingleton<PlayerRotationSingleton>();

            float rotationAngle = beamCount > 4 ? ultConfig.ValueRO.MaximumTotalDegreesPerSide / beamCount : ultConfig.ValueRO.degreesBetweenBeams;
            
            float angleToRotate = rotationAngle * i;

            if (i % 2 == 0)
                angleToRotate *= -1;
            
            quaternion rotation = quaternion.RotateY(math.radians(angleToRotate));

            float3 forwardInLocalSpace = playerRot.Forward;
            float3 forwardInGlobalSpace = math.rotate(rotation, forwardInLocalSpace);

            quaternion additionalRotation = quaternion.AxisAngle(math.up(), math.radians(angleToRotate));

            // Combine the player's rotation with the additional rotation
            quaternion newRotation = math.mul(playerRot.Value, additionalRotation);
            var beamEntity = state.EntityManager.Instantiate(ultConfig.ValueRO.BeamEntityPrefab);

            LocalTransform beamTransform = new LocalTransform
            {
                Position = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value,
                Rotation = newRotation,
                Scale = 1
            };
            state.EntityManager.SetComponentData(beamEntity, beamTransform);
            state.EntityManager.SetComponentData(beamEntity, new DirectionComponent {Value = forwardInGlobalSpace});

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
}

using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct SwordProjectileRecorderSystem : ISystem
{
    private bool _hasRecordingStarted;
    private bool _isRecording;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SwordComponent>();
        state.RequireForUpdate<ShouldRecordSwordTrajectoryComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var recorder = SystemAPI.GetSingletonRW<SwordProjectileRecorderComponent>();
        var recorderBuffer = SystemAPI.GetSingletonBuffer<SwordTrajectoryRecordingElement>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var swordEntity = SystemAPI.GetSingletonEntity<SwordComponent>();
        var swordTransform = state.EntityManager.GetComponentData<LocalTransform>(swordEntity);

        if (!_hasRecordingStarted && state.EntityManager.HasComponent<WeaponIsAttacking>(swordEntity))
        {
            _isRecording = true;
            _hasRecordingStarted = true;
        }

        if (_isRecording)
        {
            if (!state.EntityManager.HasComponent<WeaponIsAttacking>(swordEntity))
            {
                _isRecording = false;
                ecb.RemoveComponent<ShouldRecordSwordTrajectoryComponent>(swordEntity);
            }
        }
    }
}

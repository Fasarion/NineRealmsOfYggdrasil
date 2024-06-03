using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct SwordProjectileRecorderSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<SwordComponent>();
        state.RequireForUpdate<ShouldRecordSwordTrajectoryComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var recorder = SystemAPI.GetSingletonRW<SwordProjectileRecorderComponent>();

        var swordEntity = SystemAPI.GetSingletonEntity<SwordComponent>();
        var swordLocalToWorld = state.EntityManager.GetComponentData<LocalToWorld>(swordEntity);
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerLocalToWorld = state.EntityManager.GetComponentData<LocalToWorld>(playerEntity);

        recorder.ValueRW.CurrentRecordingTime += SystemAPI.Time.DeltaTime;
        
        if (recorder.ValueRO.CurrentRecordingTime > recorder.ValueRO.RecordingTime)
        {
            state.EntityManager.RemoveComponent<ShouldRecordSwordTrajectoryComponent>(swordEntity);
        }

        var playerInverse = math.inverse(playerLocalToWorld.Value);
        var pos = swordLocalToWorld.Position;
        var localPos = math.transform(playerInverse, pos);

        var element = new SwordTrajectoryRecordingElement
        {
            Position = localPos,
            Rotation = swordLocalToWorld.Rotation,
        };
        var recorderBuffer = SystemAPI.GetSingletonBuffer<SwordTrajectoryRecordingElement>();
        recorderBuffer.Add(element);
    }
}

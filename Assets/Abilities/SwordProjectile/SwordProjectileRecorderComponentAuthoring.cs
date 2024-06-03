using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SwordProjectileRecorderComponentAuthoring : MonoBehaviour
{
    public bool hasRecorded;
    public float recordingTime;
    public float currentRecordingTime;

    public class SwordProjectileRecorderComponentAuthoringBaker : Baker<SwordProjectileRecorderComponentAuthoring>
    {
        public override void Bake(SwordProjectileRecorderComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new SwordProjectileRecorderComponent
                {
                    HasRecorded = authoring.hasRecorded,
                    RecordingTime = authoring.recordingTime,
                    CurrentRecordingTime = authoring.currentRecordingTime,
                });
            AddBuffer<SwordTrajectoryRecordingElement>(entity);
        }
    }
}

public struct SwordProjectileRecorderComponent : IComponentData
{
    public bool HasRecorded;
    public float RecordingTime;
    public float CurrentRecordingTime;
}

public struct SwordTrajectoryRecordingElement : IBufferElementData
{
    public float3 Position;
    public quaternion Rotation;
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SwordProjectileRecorderComponentAuthoring : MonoBehaviour
{
    public float hasRecorded;

    public class SwordProjectileRecorderComponentAuthoringBaker : Baker<SwordProjectileRecorderComponentAuthoring>
    {
        public override void Bake(SwordProjectileRecorderComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new SwordProjectileRecorderComponent { HasRecorded = authoring.hasRecorded });
            AddBuffer<SwordTrajectoryRecordingElement>(entity);
        }
    }
}

public struct SwordProjectileRecorderComponent : IComponentData
{
    public float HasRecorded;
}

public struct SwordTrajectoryRecordingElement : IBufferElementData
{
    public float3 Position;
    public float3 Rotation;
}

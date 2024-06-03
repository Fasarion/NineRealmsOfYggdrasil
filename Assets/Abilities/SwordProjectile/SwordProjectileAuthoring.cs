using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SwordProjectileAuthoring : MonoBehaviour
{
    public int currentTransformFrame;
    public int bufferLength;
    public bool hasTarget;
    public float3 basePosition;
    public float offset;
    public bool isInitialized;
    public quaternion baseRotation;

    public class SwordProjectileAuthoringBaker : Baker<SwordProjectileAuthoring>
    {
        public override void Bake(SwordProjectileAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new SwordProjectile
                {
                    CurrentTransformFrame = authoring.currentTransformFrame,
                    BufferLength = authoring.bufferLength,
                    HasTarget = authoring.hasTarget,
                    BasePosition = authoring.basePosition,
                    Offset = authoring.offset,
                    IsInitialized = authoring.isInitialized,
                    BaseRotation = authoring.baseRotation,
                });
        }
    }
}

public struct SwordProjectile : IComponentData
{
    public int CurrentTransformFrame;
    public int BufferLength;
    public bool HasTarget;
    public float3 BasePosition;
    public float Offset;
    public bool IsInitialized;
    public quaternion BaseRotation;
}
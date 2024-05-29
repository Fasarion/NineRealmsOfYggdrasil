using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class VisualEffectComponentAuthoring : MonoBehaviour
{
    [HideInInspector] public Entity entityToFollow;
    public bool shouldLoop;
    public float activeTime;
    public float3 spawnOffset;
    public bool shouldFollowParentEntity;
    [HideInInspector] public float timerTime;

    public class VisualEffectComponentAuthoringBaker : Baker<VisualEffectComponentAuthoring>
    {
        public override void Bake(VisualEffectComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new VisualEffectComponent
                {
                    EntityToFollow = authoring.entityToFollow,
                    ShouldLoop = authoring.shouldLoop,
                    ActiveTime = authoring.activeTime,
                    SpawnOffset = authoring.spawnOffset,
                    ShouldFollowParentEntity = authoring.shouldFollowParentEntity,
                    TimerTime = authoring.timerTime
                });
        }
    }
}

public struct VisualEffectComponent : IComponentData
{
    public Entity EntityToFollow;
    public bool ShouldLoop;
    public float ActiveTime;
    public float3 SpawnOffset;
    public bool ShouldFollowParentEntity;
    public float TimerTime;
}



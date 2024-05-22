using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SeekTargetAuthoring : MonoBehaviour
{
    [Header("Distance")]
    [Tooltip("Closest distance for an entity to be considered a target when seeking for a new target.")]
    [SerializeField] private float minDistanceForFindingTarget = 0;
    
    [Tooltip("Furthest distance for an entity to be considered a target when seeking for a new target.")]
    [SerializeField] private float maxDistanceForFindingTarget;

    [Header("Field of View")]
    [Range(0, 360)]
    [Tooltip(
        "Whats the field of this entity when chasing a target? For example: a FOV of 180 degrees means the entity can " +
        "only seek targets in front of it, while 360 degrees means it can chase targets directly behind it.")]
    [SerializeField]
    private float fov = 360f;
    
    class Baker : Baker<SeekTargetAuthoring>
    {
        public override void Bake(SeekTargetAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new SeekTargetComponent
            {
                MinDistanceForSeek = authoring.minDistanceForFindingTarget,
                HalfMaxDistance = authoring.maxDistanceForFindingTarget * 0.5f,
                   
                FovInRadians = math.radians(authoring.fov) * 0.5f
            });
            
            AddComponent(entity, new HasSeekTargetEntity());
            SetComponentEnabled<HasSeekTargetEntity>(entity, false);
        }
    }
}

public struct SeekTargetComponent : IComponentData
{
    public Entity LastTargetEntity;
    
    public float MinDistanceForSeek;
    public float HalfMaxDistance;

    public float FovInRadians;
}

public struct HasSeekTargetEntity : IComponentData, IEnableableComponent
{
    public Entity TargetEntity;
}

public struct DoNextFrame : IComponentData { }
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SeekTargetAuthoring : MonoBehaviour
{
    [Header("Distance")]
    [Tooltip("Minimum distance for an to be considered a target.")]
    [SerializeField] private float minDistance = 0;
    
    [Tooltip("Maximum distance for an to be considered a target.")]
    [SerializeField] private float maxDistance;

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
                MinDistance = authoring.minDistance,
                HalfMaxDistance = authoring.maxDistance * 0.5f,
                   
                FovInRadians = math.radians(authoring.fov) * 0.5f
            });
        }
    }
}

public struct SeekTargetComponent : IComponentData
{
    public Entity TargetEntity;
    
    public float MinDistance;
    public float HalfMaxDistance;

    public float FovInRadians;
}

public struct DoNextFrame : IComponentData
{
}
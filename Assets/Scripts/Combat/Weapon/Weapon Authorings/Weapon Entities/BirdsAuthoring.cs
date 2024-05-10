using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BirdsAuthoring : MonoBehaviour
{
    [Header("Movement Options")] 
    [Tooltip("How much time it should take for the bird to come back to the player once it has started moving.")]
    [SerializeField] private float timeToCompleteMotion = 1;
    
    [Tooltip("First point on a bezier curve that decides the birds movement.")]
    [SerializeField] private float2 controlPoint1;
    
    [Tooltip("Second point on a bezier curve that decides the birds movement.")]
    [SerializeField] private float2 controlPoint2;

    private void OnValidate()
    {
        if (timeToCompleteMotion <= 0)
        {
            timeToCompleteMotion = 1;
            Debug.LogWarning("'Time To Complete Motion' must have a positive value.");
        }
    }

    class Baker : Baker<BirdsAuthoring>
    {
        public override void Bake(BirdsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var cp1 = authoring.controlPoint1;
            var cp2 = authoring.controlPoint2;
            
            AddComponent(entity, new BirdsComponent{});
            AddComponent(entity, new BirdsMovementSettingsComponent
            {
                controlPoint1 = new float4(cp1.x, 0, cp1.y, 1),
                controlPoint2 = new float4(cp2.x, 0, cp2.y, 1),
                timeToCompleteMovement = authoring.timeToCompleteMotion
            });
        }
    }
}

public struct BirdsComponent :  IComponentData{}

public struct BirdsMovementSettingsComponent : IComponentData
{
    public float4 controlPoint1;
    public float4 controlPoint2;
    public float timeToCompleteMovement;
}
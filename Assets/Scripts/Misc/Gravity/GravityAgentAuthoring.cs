using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GravityAgentAuthoring : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 1f;
    [SerializeField] private float targetYPosition = 0.5f;
    
    class Baker : Baker<GravityAgentAuthoring>
    {
        public override void Bake(GravityAgentAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GravityComponent{
                FallSpeed = authoring.fallSpeed,
                TargetYValue = authoring.targetYPosition
            });
            
            SetComponentEnabled<GravityComponent>(entity, true);
        }
    }
}

public struct GravityComponent : IComponentData, IEnableableComponent
{
    public float FallSpeed;
    public float TargetYValue;
}
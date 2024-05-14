using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Damage
{
    public class HitHandlerAuthoring : MonoBehaviour
    {
        [Header("Hit Buffer")]
        [SerializeField] private bool hasHitBuffer = true;

        [Header("Trigger Settings")]
        [SerializeField] private bool isHitTrigger;
        [SerializeField] private bool isHitTriggerTarget;
        
        [Header("Collider Settings")]
        [SerializeField] private bool isHitCollider;
        [SerializeField] private bool isHitColliderTarget;
        
        public class Baker : Baker<HitHandlerAuthoring>
        {
            public override void Bake(HitHandlerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                if (authoring.hasHitBuffer) AddBuffer<HitBufferElement>(entity);
                
                if (authoring.isHitTrigger) AddComponent<HitTriggerComponent>(entity);
                if (authoring.isHitTriggerTarget) AddComponent<HitTriggerTargetComponent>(entity);
                
                if (authoring.isHitCollider) AddComponent<HitColliderComponent>(entity);
                if (authoring.isHitColliderTarget) AddComponent<HitColliderTargetComponent>(entity);
            }
        }
    }
    
    public struct HitBufferElement : IBufferElementData
    {
        public bool IsHandled;
        public float3 Position;
        public float2 Normal;
        public Entity HitEntity;
    }

    public struct HitTriggerComponent : IComponentData{}
    public struct HitTriggerTargetComponent : IComponentData{}
    public struct HitColliderComponent : IComponentData{}
    public struct HitColliderTargetComponent : IComponentData{}
}


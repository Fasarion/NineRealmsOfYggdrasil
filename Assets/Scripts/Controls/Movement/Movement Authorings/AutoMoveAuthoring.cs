using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Movement
{
    public class AutoMoveAuthoring : MonoBehaviour
    {
        [Tooltip("This will force the object to always move in its forward direction (z-axis)")]
        [SerializeField] private bool alwaysMoveForward;

        class Baker : Baker<AutoMoveAuthoring>
        {
            public override void Bake(AutoMoveAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
            
                AddComponent(entity, new DirectionComponent());
                AddComponent(entity, new AutoMove {MoveForward = authoring.alwaysMoveForward});
            }
        }
    }

    public struct AutoMove : IComponentData
    {
        public bool MoveForward;
    }

    public struct MoveSpeedComponent : IComponentData
    {
        public float Value;
    }

    public struct DirectionComponent : IComponentData
    {
        public DirectionComponent(float3 value)
        {
            Value = value;
        }
    
        public float3 Value;
    }
}


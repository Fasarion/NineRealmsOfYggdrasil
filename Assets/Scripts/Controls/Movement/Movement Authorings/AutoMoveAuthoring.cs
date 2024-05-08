using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace Movement
{
    public class AutoMoveAuthoring : MonoBehaviour
    {
        [Tooltip("This will force the object to always move in its forward direction (its' z-axis)")]
        [SerializeField] private bool alwaysMoveForward;
        
        [Range(0,1)]
        [SerializeField] private float rotationLerpSpeed = 0.5f;

        class Baker : Baker<AutoMoveAuthoring>
        {
            public override void Bake(AutoMoveAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
            
                AddComponent(entity, new DirectionComponent());
                AddComponent(entity, new AutoMoveComponent
                {
                    MoveForward = authoring.alwaysMoveForward,
                    rotationSpeed = authoring.rotationLerpSpeed
                });
            }
        }
    }
}


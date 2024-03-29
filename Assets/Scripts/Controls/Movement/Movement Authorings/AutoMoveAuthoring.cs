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

        class Baker : Baker<AutoMoveAuthoring>
        {
            public override void Bake(AutoMoveAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
            
                AddComponent(entity, new DirectionComponent());
                AddComponent(entity, new AutoMoveComponent {MoveForward = authoring.alwaysMoveForward});
            }
        }
    }
}


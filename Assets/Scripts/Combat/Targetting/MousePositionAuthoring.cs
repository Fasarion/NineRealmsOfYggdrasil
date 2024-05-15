using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MousePositionAuthoring : MonoBehaviour
{
    class Baker : Baker<MousePositionAuthoring>
    {
        public override void Bake(MousePositionAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<MousePositionComponent>(entity);
        }
    }
}

public struct MousePositionComponent : IComponentData{}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UseMousePositionAuthoring : MonoBehaviour
{
    public class UseMousePositionAuthoringBaker : Baker<UseMousePositionAuthoring>
    {
        public override void Bake(UseMousePositionAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UseMousePosition());
        }
    }
}

public struct UseMousePosition : IComponentData
{
}

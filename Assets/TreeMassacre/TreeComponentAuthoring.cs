using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TreeComponentAuthoring : MonoBehaviour
{
    public class TreeComponentAuthoringBaker : Baker<TreeComponentAuthoring>
    {
        public override void Bake(TreeComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TreeComponent());
        }
    }
}

public struct TreeComponent : IComponentData
{
}

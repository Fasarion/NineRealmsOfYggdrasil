using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TreeBranchComponentAuthoring : MonoBehaviour
{
    public class TreeBranchComponentAuthoringBaker : Baker<TreeBranchComponentAuthoring>
    {
        public override void Bake(TreeBranchComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TreeBranchComponent());
        }
    }
}

public struct TreeBranchComponent : IComponentData
{
}

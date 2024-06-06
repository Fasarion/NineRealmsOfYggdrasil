using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class WinComponentAuthoring : MonoBehaviour
{
    public class WinComponentAuthoringBaker : Baker<WinComponentAuthoring>
    {
        public override void Bake(WinComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new WinComponent());
        }
    }
}

public struct WinComponent : IComponentData
{
}

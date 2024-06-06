using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DontUpdateAuthoring : MonoBehaviour
{
    public class DontUpdateAuthoringBaker : Baker<DontUpdateAuthoring>
    {
        public override void Bake(DontUpdateAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DontUpdate());
        }
    }
}

public struct DontUpdate : IComponentData
{
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HasBeenThunderStruckComponentAuthoring : MonoBehaviour
{
    public class HasBeenThunderStruckComponentAuthoringBaker : Baker<HasBeenThunderStruckComponentAuthoring>
    {
        public override void Bake(HasBeenThunderStruckComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HasBeenThunderStruckComponent());
        }
    }
}

public struct HasBeenThunderStruckComponent : IComponentData
{
}

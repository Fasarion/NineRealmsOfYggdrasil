using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AbilityTagComponentAuthoring : MonoBehaviour
{
    public class AbilityTagComponentAuthoringBaker : Baker<AbilityTagComponentAuthoring>
    {
        public override void Bake(AbilityTagComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AbilityTagComponent());
        }
    }
}

public struct AbilityTagComponent : IComponentData
{
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class IsUnlockedAuthoring : MonoBehaviour
{
    public class IsUnlockedAuthoringBaker : Baker<IsUnlockedAuthoring>
    {
        public override void Bake(IsUnlockedAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new IsUnlocked());
        }
    }
}

public struct IsUnlocked : IComponentData
{
}

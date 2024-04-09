using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HasOwnerAuthoring : MonoBehaviour
{
    public class Baker:Baker<HasOwnerAuthoring>
    {
        public override void Bake(HasOwnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new OwnerEntity());
        }
    }
}

public struct OwnerEntity : IComponentData
{
    public Entity Value;
}

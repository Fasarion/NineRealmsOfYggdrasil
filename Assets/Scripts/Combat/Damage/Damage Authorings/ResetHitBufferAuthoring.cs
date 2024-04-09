using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ResetHitBufferAuthoring : MonoBehaviour
{
    class Baker : Baker<ResetHitBufferAuthoring>
    {
        public override void Bake(ResetHitBufferAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ResetHitBufferComponent());
        }
    }
}

public struct ResetHitBufferComponent : IComponentData
{
}
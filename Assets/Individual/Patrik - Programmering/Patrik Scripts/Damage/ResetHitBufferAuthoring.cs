using System.Collections;
using System.Collections.Generic;
using Damage;
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

public partial struct ResetHitBufferEveryFourthFrameSystem : ISystem
{
    private int lastHitBufferFrame;
    private int currentBufferFrame;

    public void OnUpdate(ref SystemState state)
    {
        int framesBetweenResets = 4;
        currentBufferFrame++;

        if (currentBufferFrame < lastHitBufferFrame + framesBetweenResets)
            return;

        foreach (var hitBuffer in SystemAPI.Query<DynamicBuffer<HitBufferElement>>().WithAll<ResetHitBufferComponent>())
        {
            hitBuffer.Clear();
            lastHitBufferFrame = currentBufferFrame;
        }
    }
}

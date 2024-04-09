using Damage;
using Unity.Entities;

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
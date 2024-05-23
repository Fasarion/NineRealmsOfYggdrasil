using Damage;
using Unity.Entities;

public partial struct AttackStatTransferSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UpdateStatsComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        // transfer component data
        foreach (var  (updateStats, receiverEntity) in SystemAPI
            .Query<UpdateStatsComponent>().WithEntityAccess())
        {
            Entity transfererEnttiy = updateStats.EntityToTransferStatsFrom;

            TryTransferComponentData<CachedDamageComponent>(state, transfererEnttiy, receiverEntity, ecb);
            TryTransferComponentData<KnockBackOnHitComponent>(state, transfererEnttiy, receiverEntity, ecb);
            TryTransferComponentData<ShouldApplyHitStopOnHit>(state, transfererEnttiy, receiverEntity, ecb);
            TryTransferComponentData<ElementalShouldApplyFireComponent>(state, transfererEnttiy, receiverEntity, ecb);
            TryTransferComponentData<ElementalShouldApplyIceComponent>(state, transfererEnttiy, receiverEntity, ecb);
        }

        // remove update stats tags
        foreach (var (_, entity) in SystemAPI.Query<UpdateStatsComponent>()
                .WithAll<UpdateStatsComponent>().WithEntityAccess())
        {
            ecb.RemoveComponent<UpdateStatsComponent>(entity);
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private void TryTransferComponentData<T>(SystemState state, Entity transfererEnttiy, Entity receiverEntity,
        EntityCommandBuffer ecb) where T : unmanaged, IComponentData
    {
        if (state.EntityManager.HasComponent<T>(transfererEnttiy))
        {
            var transferComponent = state.EntityManager.GetComponentData<T>(transfererEnttiy);

            // Add component to receiver if it doesn't already have it
            if (!state.EntityManager.HasComponent<T>(receiverEntity))
                ecb.AddComponent<T>(receiverEntity);

            ecb.SetComponent(receiverEntity, transferComponent);
        }
    }
}
using Unity.Entities;

public partial struct AttackStatTransferSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UpdateStatsComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // transfer damage
        foreach (var (damageTranserReciever, updateStats) in SystemAPI
            .Query<RefRW<CachedDamageComponent>, UpdateStatsComponent>())
        {
            var damageToTransfer = state.EntityManager.GetComponentData<CachedDamageComponent>(updateStats.EntityToTransferStatsFrom);
            damageTranserReciever.ValueRW.Value = damageToTransfer.Value;
        }

        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
        // remove update stats tags
        foreach (var (_, entity) in SystemAPI.Query<UpdateStatsComponent>()
                .WithAll<UpdateStatsComponent>()
                .WithEntityAccess())
        {
            ecb.RemoveComponent<UpdateStatsComponent>(entity);
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
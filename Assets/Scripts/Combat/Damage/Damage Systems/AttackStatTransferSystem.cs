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

        // transfer damage
        foreach (var  (updateStats, receiverEntity) in SystemAPI
            .Query<UpdateStatsComponent>().WithEntityAccess())
        {
            Entity transfererEnttiy = updateStats.EntityToTransferStatsFrom;

            // Transfer damage
            if (state.EntityManager.HasComponent<CachedDamageComponent>(transfererEnttiy))
            {
                var transferComponent = state.EntityManager.GetComponentData<CachedDamageComponent>(transfererEnttiy);
                
                if (!state.EntityManager.HasComponent<CachedDamageComponent>(receiverEntity))
                    ecb.AddComponent<CachedDamageComponent>(receiverEntity);
                
                ecb.SetComponent(receiverEntity, transferComponent);
            }
            
            // Transfer knock back
            if (state.EntityManager.HasComponent<KnockBackOnHitComponent>(transfererEnttiy))
            {
                var transferComponent = state.EntityManager.GetComponentData<KnockBackOnHitComponent>(transfererEnttiy);
                
                if (!state.EntityManager.HasComponent<KnockBackOnHitComponent>(receiverEntity))
                    ecb.AddComponent<KnockBackOnHitComponent>(receiverEntity);
                
                ecb.SetComponent(receiverEntity, transferComponent);
            }
            
            // Transfer hit stun
            if (state.EntityManager.HasComponent<ShouldApplyHitStopOnHit>(transfererEnttiy))
            {
                var transferComponent = state.EntityManager.GetComponentData<ShouldApplyHitStopOnHit>(transfererEnttiy);
                
                if (!state.EntityManager.HasComponent<ShouldApplyHitStopOnHit>(receiverEntity))
                    ecb.AddComponent<ShouldApplyHitStopOnHit>(receiverEntity);
                
                ecb.SetComponent(receiverEntity, transferComponent);
            }
        }
        
        // // transfer damage
        // foreach (var (damageTransferReceiver, updateStats) in SystemAPI
        //     .Query<RefRW<CachedDamageComponent>, UpdateStatsComponent>())
        // {
        //     var damageToTransfer = state.EntityManager.GetComponentData<CachedDamageComponent>(updateStats.EntityToTransferStatsFrom);
        //     damageTransferReceiver.ValueRW.Value = damageToTransfer.Value;
        // }
        //
        // // transfer hit stun
        // foreach (var (damageTransferReceiver, updateStats) in SystemAPI
        //     .Query<RefRW<ShouldApplyHitStopOnHit>, UpdateStatsComponent>())
        // {
        //     Entity receiverEntity = updateStats.EntityToTransferStatsFrom;
        //
        //     if (!state.EntityManager.HasComponent<ShouldApplyHitStopOnHit>(receiverEntity))
        //     {
        //         ecb.AddComponent<ShouldApplyHitStopOnHit>(receiverEntity);
        //     }
        //     
        //     var damageToTransfer = state.EntityManager.GetComponentData<CachedDamageComponent>(updateStats.EntityToTransferStatsFrom);
        //     damageTransferReceiver.ValueRW.Value = damageToTransfer.Value;
        // }

        
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
using Destruction;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(PlayerRotationSystem))]
public partial struct HammerSpecialIndicationSystem : ISystem
{
    private bool hasInitialized;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<PlayerPositionSingleton>();
        state.RequireForUpdate<PlayerRotationSingleton>();
        
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<HammerComponent>();
        state.RequireForUpdate<HammerSpecialConfig>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

        if (!attackCaller.ValueRO.SpecialChargeInfo.IsChargingWithWeapon(WeaponType.Hammer))
        {
            if (hasInitialized)
            {
                var ecb = new EntityCommandBuffer(Allocator.Temp);
                
                foreach (var (indicator, transform, chargeTimer, entity) in
                    SystemAPI.Query<RefRW<HammerChargeComponent>, RefRW<LocalTransform>, RefRW<ChargeTimer>>()
                        .WithEntityAccess())
                {
                    ecb.AddComponent<ShouldBeDestroyed>(entity);
                }
                
                ecb.Playback(state.EntityManager);
                ecb.Dispose();

                hasInitialized = false;
            }

            return;
        }

        // initialize indicator
        if (!hasInitialized)
        {
            var config = SystemAPI.GetSingleton<HammerSpecialConfig>();
            
            var ability = state.EntityManager.Instantiate(config.IndicatorPrefab);
            state.EntityManager.SetComponentData(ability, new ChargeTimer
            {
                maxChargeTime = 3
            });

            hasInitialized = true;
        }

        // make indicator follow player
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>();
        var playerRot = SystemAPI.GetSingleton<PlayerRotationSingleton>();
        foreach (var (indicator, transform, chargeTimer, entity) in
            SystemAPI.Query<RefRW<HammerChargeComponent>, RefRW<LocalTransform>, RefRW<ChargeTimer>>()
                .WithEntityAccess())
        {
            transform.ValueRW.Position = playerPos.Value;
            transform.ValueRW.Rotation = playerRot.Value;
        }

    }
}
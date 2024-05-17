using Movement;
using Patrik;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Weapon;


[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct HammerPassiveAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HammerPassiveAbilityConfig>();
        state.RequireForUpdate<HammerComponent>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<GameUnpaused>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        if (!attackCaller.ValueRO.ShouldStartPassiveAttack(WeaponType.Hammer))
        {
            return;
        }

        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var config = SystemAPI.GetSingleton<HammerPassiveAbilityConfig>();
        state.EntityManager.Instantiate(config.AbilityPrefab);

        foreach (var (_, entity) in
                 SystemAPI.Query<HasBeenThunderStruckComponent>()
                     .WithEntityAccess())
        {
            ecb.RemoveComponent<HasBeenThunderStruckComponent>(entity);
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
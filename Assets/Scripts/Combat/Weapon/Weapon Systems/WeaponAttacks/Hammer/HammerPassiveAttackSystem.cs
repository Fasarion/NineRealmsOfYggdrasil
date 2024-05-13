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
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        if (!attackCaller.ValueRO.ShouldStartPassiveAttack(WeaponType.Hammer))
        {
            return;
        }

        var config = SystemAPI.GetSingleton<HammerPassiveAbilityConfig>();
        state.EntityManager.Instantiate(config.AbilityPrefab);
    }
}
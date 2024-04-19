using System.Collections;
using System.Collections.Generic;
using Damage;
using Patrik;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public partial struct SwordSpecialAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SwordStatsTag>();
        state.RequireForUpdate<BasePlayerStatsTag>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<SwordComponent>();
        state.RequireForUpdate<AudioBufferData>();
        state.RequireForUpdate<IceRingConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        
        if (!attackCaller.ValueRO.ShouldActiveAttackWithType(WeaponType.Sword, AttackType.Special))
            return;

        attackCaller.ValueRW.StartActiveAttackData.Enabled = false;
        
        
        var config = SystemAPI.GetSingleton<IceRingConfig>();
        
        var query = SystemAPI.QueryBuilder().WithAll<IceRingConfig, ChargeTimer>().Build();
        if (query.CalculateEntityCount() == 0)
        {
            var ability = state.EntityManager.Instantiate(config.chargeAreaPrefab);
            state.EntityManager.SetComponentData(ability, new ChargeTimer
            {
                maxChargeTime = 3
            });
            
        }
    }
}

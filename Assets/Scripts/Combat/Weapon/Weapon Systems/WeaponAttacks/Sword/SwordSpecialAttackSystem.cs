using System.Collections;
using System.Collections.Generic;
using Damage;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(CombatStatHandleSystem))]
public partial struct SwordSpecialAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
      //  state.RequireForUpdate<SwordStatsTag>();
     //   state.RequireForUpdate<BasePlayerStatsTag>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<SwordComponent>();
        state.RequireForUpdate<AudioBufferData>();
        state.RequireForUpdate<IceRingConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        
        if (attackCaller.ValueRO.SpecialChargeInfo.chargeState != ChargeState.Start)
            return;
        
        if (attackCaller.ValueRO.SpecialChargeInfo.ChargingWeapon != WeaponType.Sword)
            return;
        
        if (attackCaller.ValueRO.ActiveAttackData.IsAttacking)
            return;

        attackCaller.ValueRW.ActiveAttackData.ShouldStart = false;
        
        
        var query = SystemAPI.QueryBuilder().WithAll<IceRingAbility, ChargeTimer>().Build();
        
        if (query.CalculateEntityCount() == 0)
        {
            var config = SystemAPI.GetSingleton<IceRingConfig>();
        
            var configEntity = SystemAPI.GetSingletonEntity<IceRingConfig>();
            var abilityDamage = state.EntityManager.GetComponentData<CachedDamageComponent>(configEntity);
            var swordEntity = SystemAPI.GetSingletonEntity<SwordComponent>();
            var swordDamage = state.EntityManager.GetComponentData<CachedDamageComponent>(swordEntity);
            abilityDamage.Value = swordDamage.Value;
            state.EntityManager.SetComponentData(configEntity, abilityDamage);
            
            var ability = state.EntityManager.Instantiate(config.chargeAreaPrefab);
            state.EntityManager.SetComponentData(ability, new ChargeTimer
            {
                maxChargeTime = 3
            });
        }
    }
}

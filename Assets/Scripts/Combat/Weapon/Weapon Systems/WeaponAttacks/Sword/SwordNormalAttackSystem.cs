using System.Collections;
using System.Collections.Generic;
using Patrik;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(SwordUltimateAttackSystem))]
[UpdateAfter(typeof(SwordSpecialAttackSystem))]
[BurstCompile]
public partial struct SwordNormalAttackSystem : ISystem
{
   // private bool hasRecorded;
    private int attackType;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SwordComboAbilityConfig>();
        state.RequireForUpdate<SwordComponent>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<GameUnpaused>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        var config = SystemAPI.GetSingleton<SwordComboAbilityConfig>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
        if (attackCaller.ValueRO.ShouldStartActiveAttack(WeaponType.Sword, AttackType.Normal))
        {
            if (attackType == 1)
            {
                var configEntity = SystemAPI.GetSingletonEntity<SwordComboAbilityConfig>();
                if (state.EntityManager.HasComponent<IsUnlocked>(configEntity))
                {
                    state.EntityManager.Instantiate(config.SwordComboAbilityPrefab);
                }
            }
            else
            {
                foreach (var (_, entity) in
                         SystemAPI.Query<SwordProjectileTarget>()
                             .WithEntityAccess())
                {
                    ecb.RemoveComponent<SwordProjectileTarget>(entity);
                }
            }
            
            if (attackType == 0) attackType = 1;
            else attackType = 0;
            
            attackCaller.ValueRW.ActiveAttackData.ShouldStart = false;
            if (!config.HasRecorded)
            {
                var configRW = SystemAPI.GetSingletonRW<SwordComboAbilityConfig>();
                
                configRW.ValueRW.HasRecorded = true;
                var swordEntity = SystemAPI.GetSingletonEntity<SwordComponent>();
                state.EntityManager.AddComponent<ShouldRecordSwordTrajectoryComponent>(swordEntity);
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

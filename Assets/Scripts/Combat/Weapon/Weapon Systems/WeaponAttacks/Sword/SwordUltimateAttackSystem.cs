using System.Collections;
using System.Collections.Generic;
using Patrik;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(HandleAnimationSystem))]
public partial struct SwordUltimateAttackSystem : ISystem
{
    private int _attackCount;
    private bool _isActive;

    private float scaleChangeFactor;
    private float numberOfScaledAttacks;
    

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SwordStatsTag>();
        state.RequireForUpdate<BasePlayerStatsTag>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<SwordComponent>();
        state.RequireForUpdate<AudioBufferData>();

        scaleChangeFactor = 2;
        numberOfScaledAttacks = 2;
    }


    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        var swordEntity = SystemAPI.GetSingletonEntity<SwordStatsTag>();

        if (_isActive)
        {
            bool weaponSwitch = false;
            if (attackCaller.ValueRO.ShouldActiveAttackWithType(WeaponType.Sword, AttackType.Normal) || weaponSwitch)
            {
                _attackCount++;
                
                if (_attackCount > numberOfScaledAttacks)
                {
                    var weaponStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(swordEntity);
                    weaponStatsComponent.NormalAttackStats.Size.BaseValue /= scaleChangeFactor;
                    
                    state.EntityManager.SetComponentData(swordEntity, weaponStatsComponent);
                    
                    var statHandler = SystemAPI.GetSingletonRW<StatHandlerComponent>();
                    statHandler.ValueRW.ShouldUpdateStats = true;
                
                    _isActive = false;
                }
            }
        }
        
        
        if (!attackCaller.ValueRO.ShouldActiveAttackWithType(WeaponType.Sword, AttackType.Ultimate))
            return;
        
        attackCaller.ValueRW.StartActiveAttackData.Enabled = false;
        
        // Initialize attack
        if (!_isActive)
        {
            var weaponStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(swordEntity);
            weaponStatsComponent.NormalAttackStats.Size.BaseValue *= scaleChangeFactor;
                    
            state.EntityManager.SetComponentData(swordEntity, weaponStatsComponent);
            
            var statHandler = SystemAPI.GetSingletonRW<StatHandlerComponent>();
            statHandler.ValueRW.ShouldUpdateStats = true;

            _isActive = true;
            _attackCount = 0;
        }




    }

}

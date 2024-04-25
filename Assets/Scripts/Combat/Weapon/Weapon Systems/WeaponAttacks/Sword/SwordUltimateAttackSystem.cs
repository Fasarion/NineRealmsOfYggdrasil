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
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<SwordComponent>();
        state.RequireForUpdate<AudioBufferData>();

        // TODO: put in config
        scaleChangeFactor = 2;
        numberOfScaledAttacks = 3;
    }


    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        var swordEntity = SystemAPI.GetSingletonEntity<SwordComponent>();
        
        if (_isActive)
        {
            bool stoppedSwordAttack = attackCaller.ValueRO.ActiveAttackData.ShouldStopAttack(WeaponType.Sword) ||
                                      attackCaller.ValueRO.PassiveAttackData.ShouldStopAttack(WeaponType.Sword);
            if (stoppedSwordAttack)
            {
                _attackCount++;
                
                if (_attackCount > numberOfScaledAttacks)
                {
                    var scaleComp = state.EntityManager.GetComponentData<SizeComponent>(swordEntity);
                    scaleComp.Value -= scaleChangeFactor;
                    state.EntityManager.SetComponentData(swordEntity, scaleComp);

                    var statHandler = SystemAPI.GetSingletonRW<StatHandlerComponent>();
                    statHandler.ValueRW.ShouldUpdateStats = true;
                
                    _isActive = false;
                }
            }
        }
        
        
        if (!attackCaller.ValueRO.ShouldStartActiveAttack(WeaponType.Sword, AttackType.Ultimate))
            return;
        
        attackCaller.ValueRW.ActiveAttackData.ShouldStart = false;
        
        // Initialize attack
        if (!_isActive)
        {
            var scaleComp = state.EntityManager.GetComponentData<SizeComponent>(swordEntity);

            float newSize = scaleComp.Value += scaleChangeFactor;
            
            scaleComp.Value = newSize;
            state.EntityManager.SetComponentData(swordEntity, scaleComp);
            
            var statHandler = SystemAPI.GetSingletonRW<StatHandlerComponent>();
            statHandler.ValueRW.ShouldUpdateStats = true;

            _isActive = true;
            _attackCount = 0;
        }
    }
}

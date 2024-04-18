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
    private float _cachedScale;
    private Vector3 _cachedLocalTransformSize;
    

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SwordStatsTag>();
        state.RequireForUpdate<BasePlayerStatsTag>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<SwordComponent>();
        state.RequireForUpdate<AudioBufferData>();
    }


    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        
        var swordEntity = SystemAPI.GetSingletonEntity<SwordStatsTag>();
        var swordTransform = state.EntityManager.GetComponentData<LocalTransform>(swordEntity);
        var swordGO = state.EntityManager.GetComponentData<AnimatorReference>(swordEntity);
        var size = SystemAPI.GetComponent<SizeComponent>(swordEntity).Value;
        
        if (_isActive)
        {
            if (attackCaller.ValueRO.ShouldActiveAttackWithType(WeaponType.Sword, AttackType.Normal))
            {
                _attackCount++;
            }
            
            
            if (_attackCount > 2)
            {
                _isActive = false;
                swordTransform.Scale = _cachedScale;
                swordGO.Animator.transform.localScale = _cachedLocalTransformSize;
            }
        }
        
        
        if (!attackCaller.ValueRO.ShouldActiveAttackWithType(WeaponType.Sword, AttackType.Ultimate))
            return;
        
        attackCaller.ValueRW.shouldActiveAttack = false;
        
        
        if (!_isActive)
        {
            _cachedScale = swordTransform.Scale;
            _cachedLocalTransformSize = swordGO.Animator.transform.localScale;
            swordTransform.Scale = size;
            state.EntityManager.SetComponentData(swordEntity, swordTransform);
            swordGO.Animator.transform.localScale = Vector3.one * size;
            _isActive = true;
            _attackCount = 0;
        }




    }

}

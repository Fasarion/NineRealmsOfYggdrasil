using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(CombatStatHandleSystem))]
public partial struct CombatManagedStatWriter : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BasePlayerStatsTag>();
        state.RequireForUpdate<StatHandlerComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var statHandler = SystemAPI.GetSingletonRW<StatHandlerComponent>();
        if (!statHandler.ValueRO.ShouldUpdateStats)
            return;
        
        WriteOverAttackData(ref state);
    }
    
    void WriteOverAttackData(ref SystemState state)
    {
        // Write Over Scale
        var playerStatsEntity = SystemAPI.GetSingletonEntity<BasePlayerStatsTag>();
        var playerStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(playerStatsEntity);
        
        foreach (var (transform, animatorReference, weaponStatsComponent, weapon, entity) in SystemAPI
            .Query<RefRW<LocalTransform>, AnimatorReference, CombatStatsComponent, WeaponComponent>()
            .WithEntityAccess())
        {
            float size = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, weapon.CurrentAttackType, CombatStatType.Size, weapon.CurrentAttackCombo);
            
            transform.ValueRW.Scale = size;
            animatorReference.Animator.transform.localScale = Vector3.one * size;
        }

        // var playerAttackSpeed = state.EntityManager.GetComponentData<AttackSpeedModifier>(playerStatsEntity);
        //
        // // Write Over Attack Speed
        // foreach (var (attackSpeedModifier, animatorReference, entity) in SystemAPI
        //     .Query<AttackSpeedModifier, AnimatorReference>().WithEntityAccess())
        // {
        //     bool isPlayer = state.EntityManager.HasComponent<PlayerTag>(entity);
        //     if (isPlayer)
        //     {
        //         animatorReference.Animator.speed = attackSpeedModifier.Value;
        //     }
        //     else
        //     {
        //         animatorReference.Animator.speed = attackSpeedModifier.Value * playerAttackSpeed.Value;
        //     }
        // }
    }
}
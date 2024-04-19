using Patrik;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(CombatStatHandleSystem))]
public partial struct OverrideWeaponScaleSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BasePlayerStatsTag>();
        state.RequireForUpdate<StatHandlerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var statHandler = SystemAPI.GetSingletonRW<StatHandlerComponent>();
        if (!statHandler.ValueRO.ShouldUpdateStats)
            return;
        
        WriteOverAttackData(ref state);
    }
    
    void WriteOverAttackData(ref SystemState state)
    {
        Debug.Log("Write over size attack data");
        
        var playerStatsEntity = SystemAPI.GetSingletonEntity<BasePlayerStatsTag>();
        var playerStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(playerStatsEntity);


        foreach (var (transform, animatorReference, weaponStatsComponent, weapon, entity) in SystemAPI
            .Query<RefRW<LocalTransform>, AnimatorReference, CombatStatsComponent, WeaponComponent>()
            .WithEntityAccess())
        {
            float size = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, weapon.CurrentAttackType, CombatStatType.Size, weapon.CurrentAttackCombo);
            Debug.Log($"New size: {size}");
            
            transform.ValueRW.Scale = size;
            animatorReference.Animator.transform.localScale = Vector3.one * size;
        }
        
    }
}
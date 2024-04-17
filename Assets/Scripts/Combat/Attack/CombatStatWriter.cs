using Damage;
using Patrik;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct CombatStatHandleSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
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
        var statHandler = SystemAPI.GetSingletonRW<StatHandlerComponent>();

        var weaponType = statHandler.ValueRO.WeaponType;
        var attackType = statHandler.ValueRO.AttackType;
        int combo = statHandler.ValueRO.ComboCounter;
        
        var entity = GetWeaponEntity(ref state, weaponType);

        // update damage data
        var weaponStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(entity);
        var playerStatsEntity = SystemAPI.GetSingletonEntity<BasePlayerStatsTag>();
        var playerStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(playerStatsEntity);
            
        float totalDamage = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, attackType, CombatStatType.Damage, combo);
        float totalCritRate = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, attackType, CombatStatType.CriticalRate, combo);
        float totalCritMod = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, attackType, CombatStatType.CriticalModifier, combo);
            
        DamageContents damageContents = new DamageContents()
        {
            DamageValue = totalDamage,
            CriticalRate = totalCritRate,
            CriticalModifier = totalCritMod,
        };

        DamageOnTriggerComponent damageComp = state.EntityManager.GetComponentData<DamageOnTriggerComponent>(entity);
        damageComp.Value = damageContents;
        state.EntityManager.SetComponentData(entity, damageComp);
            
        // update knockback knockback
        float totalKnockBack = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, attackType, CombatStatType.KnockBack, combo);

        KnockBackForce knockBackComp = state.EntityManager.GetComponentData<KnockBackForce>(entity);
        knockBackComp.Value = totalKnockBack;
        state.EntityManager.SetComponentData(entity, knockBackComp);

        statHandler.ValueRW.ShouldUpdateStats = false;
    }
    
    private Entity GetWeaponEntity(ref SystemState state, WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword:
                foreach (var (sword, entity) in SystemAPI.Query<SwordComponent>()
                    .WithEntityAccess())
                {
                    return entity;
                }
                break;
                
            case WeaponType.Hammer:
                foreach (var (hammer, entity) in SystemAPI.Query<HammerComponent>()
                    .WithEntityAccess())
                {
                    return entity;
                }
                break;
        }

        return default;
    }
}
using Damage;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public partial struct CombatStatHandleSystem : ISystem
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
    
    [BurstCompile]
    void WriteOverAttackData(ref SystemState state)
    {
        var statHandler = SystemAPI.GetSingletonRW<StatHandlerComponent>();

        var weaponType = statHandler.ValueRO.WeaponType;
        var attackType = statHandler.ValueRO.AttackType;
        int combo = statHandler.ValueRO.ComboCounter;
        
        var weaponEntity = GetWeaponEntity(ref state, weaponType);

        // update damage data
        var weaponStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(weaponEntity);
        var playerStatsEntity = SystemAPI.GetSingletonEntity<BasePlayerStatsTag>();
        var playerStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(playerStatsEntity);
            
        // float totalDamage = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, attackType, CombatStatType.Damage, combo);
        // float totalCritRate = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, attackType, CombatStatType.CriticalRate, combo);
        // float totalCritMod = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, attackType, CombatStatType.CriticalModifier, combo);
        
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerDamageMod = state.EntityManager.GetComponentData<DamageModifierComponent>(playerEntity);
        var playerSkillMod = state.EntityManager.GetComponentData<SkillModifierComponent>(playerEntity);
        var playerDamageComp = state.EntityManager.GetComponentData<DamageComponent>(playerEntity);
        
      //  var baseWeaponDmgComponent = state.EntityManager.GetComponentData<BaseAttackDamageComponent>(weaponEntity);

        foreach (var (baseWeaponDmgComponent, currDamageComp, damageModifier, skillModifier, weapon) in SystemAPI
            .Query<DamageComponent, RefRW<CachedDamageComponent>,  DamageModifierComponent, SkillModifierComponent, WeaponComponent>())
        {
            AttackType weaponAttack = weapon.CurrentAttackType;
            
            float totalDamage = (playerDamageComp.Value.DamageValue + baseWeaponDmgComponent.Value.DamageValue)
                                * (playerDamageMod.Value + damageModifier.Value)
                                * (playerSkillMod.Value.GetModifier(weaponAttack) + skillModifier.Value.GetModifier(weaponAttack));
            
            float totalCritRate = playerDamageComp.Value.CriticalRate + baseWeaponDmgComponent.Value.CriticalRate;

            DamageContents damageContents = new DamageContents()
            {
                DamageValue = totalDamage,
                CriticalRate = totalCritRate,
            };

            currDamageComp.ValueRW.Value = damageContents;
        }

        // float totalDamage = (playerDamageComp.Value.DamageValue + baseWeaponDmgComponent.Value.DamageValue)
        //                     * playerAtkModsComponent.DamageModifier
        //                     * playerAtkModsComponent.AttackTypeModifier.GetModifier(attackType);

        // float totalCritRate = playerDamageComp.Value.CriticalRate + baseWeaponDmgComponent.Value.CriticalRate;
        
        
        // DamageContents damageContents = new DamageContents()
        // {
        //     DamageValue = totalDamage,
        //     CriticalRate = totalCritRate,
        //    // CriticalModifier = totalCritMod,
        // };
        // //
        // var currDamageComp = state.EntityManager.GetComponentData<CurrentAttackDamageComponent>(weaponEntity);
        // currDamageComp.Value = damageContents;
        // state.EntityManager.SetComponentData(weaponEntity, currDamageComp);

        // DamageOnTriggerComponent damageComp = state.EntityManager.GetComponentData<DamageOnTriggerComponent>(weaponEntity);
        // damageComp.Value = damageContents;
        // state.EntityManager.SetComponentData(weaponEntity, damageComp);
            
        // update knockback data
        float totalKnockBack = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, attackType, CombatStatType.KnockBack, combo);

        KnockBackForce knockBackComp = state.EntityManager.GetComponentData<KnockBackForce>(weaponEntity);
        knockBackComp.Value = totalKnockBack;
        state.EntityManager.SetComponentData(weaponEntity, knockBackComp);
        
        // update energy data
        float totalEnergyFill = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent,
            attackType, CombatStatType.EnergyFillPerHit, combo);
        EnergyFillComponent energyFillComp =  state.EntityManager.GetComponentData<EnergyFillComponent>(weaponEntity);
        if (attackType == AttackType.Passive)
        {
            energyFillComp.PassiveFillPerHit = totalEnergyFill;
        }
        else
        {
            energyFillComp.ActiveFillPerHit = totalEnergyFill;
        }
        state.EntityManager.SetComponentData(weaponEntity, energyFillComp);
        
        // area
        float area = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, attackType, CombatStatType.Area, combo);
        AreaComponentData areaComponent = state.EntityManager.GetComponentData<AreaComponentData>(weaponEntity);
        areaComponent.Value = area;
        state.EntityManager.SetComponentData(weaponEntity, areaComponent);
        
        
        // // size
        // float size = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, attackType, CombatStatType.Size, combo);
        // var transform = state.EntityManager.GetComponentData<LocalTransform>(entity);
        // transform.Scale = size;
        // state.EntityManager.SetComponentData(entity, transform);
        //
        // var gameObjectReference = state.EntityManager.GetComponentData<AnimatorReference>(entity);
        // gameObjectReference.Animator.transform.localScale = Vector3.one * size;
        
        // SizeComponent sizeComponent = state.EntityManager.GetComponentData<SizeComponent>(entity);
        // sizeComponent.Value = size;
        // state.EntityManager.SetComponentData(entity, sizeComponent);
        
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
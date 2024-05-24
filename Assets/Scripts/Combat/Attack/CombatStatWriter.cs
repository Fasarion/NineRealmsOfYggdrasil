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
        state.RequireForUpdate<PlayerTag>();
        //  state.RequireForUpdate<BasePlayerStatsTag>();
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

      //  var weaponStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(weaponEntity);
      //  var playerStatsEntity = SystemAPI.GetSingletonEntity<BasePlayerStatsTag>();
       // var playerStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(playerStatsEntity);
            
        // update damage data
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerDamageMod = state.EntityManager.GetComponentData<DamageModifierComponent>(playerEntity);
        var playerSkillMod = state.EntityManager.GetComponentData<SkillModifierComponent>(playerEntity);
        var playerDamageComp = state.EntityManager.GetComponentData<DamageComponent>(playerEntity);
        
        foreach (var (baseWeaponDmgComponent, currDamageComp, damageModifier, skillModifier, weapon) in SystemAPI
            .Query<DamageComponent, RefRW<CachedDamageComponent>,  DamageModifierComponent, SkillModifierComponent, WeaponComponent>())
        {
            AttackType weaponAttack = weapon.CurrentAttackType;
            
            float totalDamage = (playerDamageComp.Value.DamageValue + baseWeaponDmgComponent.Value.DamageValue)
                                * (1 + playerDamageMod.Value + damageModifier.Value)
                                * (playerSkillMod.Value.GetModifier(weaponAttack) + skillModifier.Value.GetModifier(weaponAttack));
            
            float totalCritRate = playerDamageComp.Value.CriticalRate + baseWeaponDmgComponent.Value.CriticalRate;

            DamageContents damageContents = new DamageContents()
            {
                DamageValue = totalDamage,
                CriticalRate = totalCritRate,
            };

            currDamageComp.ValueRW.Value = damageContents;
        }
        
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        // transfer stats to abilites
        foreach (var weapon in 
            SystemAPI.Query<WeaponComponent>())
        {
            foreach (var (updateStatsFromAttack, entity) in 
                SystemAPI.Query<UpdateStatsFromAttack>()
                    .WithNone<UpdateStatsComponent>()
                    .WithEntityAccess())
            {
                if (weapon.WeaponType != updateStatsFromAttack.WeaponType) continue;
                if (weapon.CurrentAttackType != updateStatsFromAttack.AttackType) continue;

                var entityToTransferStatsFrom = GetWeaponEntity(ref state, updateStatsFromAttack.WeaponType);
            
                ecb.AddComponent<UpdateStatsComponent>(entity);
                UpdateStatsComponent updateStatsComponent = new UpdateStatsComponent
                {
                    EntityToTransferStatsFrom = entityToTransferStatsFrom
                };
                ecb.SetComponent(entity, updateStatsComponent);
            }
        }
        
        
        
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        
        statHandler = SystemAPI.GetSingletonRW<StatHandlerComponent>(); 
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
            
            case WeaponType.Birds:
                foreach (var (hammer, entity) in SystemAPI.Query<BirdsComponent>()
                    .WithEntityAccess())
                {
                    return entity;
                }
                break;
        }

        return default;
    }
}
using System.Collections;
using System.Collections.Generic;
using Damage;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateBefore(typeof(IceRingSystem))]
[BurstCompile]
public partial struct AbilityStatWriter : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<ShouldSetDamageValuesComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerDamageMod = state.EntityManager.GetComponentData<DamageModifierComponent>(playerEntity);
        var playerSkillMod = state.EntityManager.GetComponentData<SkillModifierComponent>(playerEntity);
        var playerDamageComponent = state.EntityManager.GetComponentData<DamageComponent>(playerEntity);
        
        foreach (var (cachedDamage, shouldSetDamageComponent, entity) in SystemAPI
                     .Query<RefRW<CachedDamageComponent>, ShouldSetDamageValuesComponent>().WithEntityAccess())
        {
            var abilityType = shouldSetDamageComponent.AttackType;
            var weaponType = shouldSetDamageComponent.WeaponType;

            var weaponEntity = GetWeaponEntity(ref state, weaponType);

            var baseWeaponDmgComponent = GetDamageComponent(ref state, weaponEntity);
            var damageModifier = GetDamageModifierComponent(ref state, weaponEntity);
            var skillModifier = GetSkillModifier(ref state, weaponEntity, abilityType);
            
            float totalDamage = (playerDamageComponent.Value.DamageValue + baseWeaponDmgComponent.Value.DamageValue)
                                * (1 + playerDamageMod.Value + damageModifier.Value)
                                * (playerSkillMod.Value.GetModifier(abilityType) + skillModifier.Value.GetModifier(abilityType));
            
            float totalCritRate = playerDamageComponent.Value.CriticalRate + baseWeaponDmgComponent.Value.CriticalRate;

            DamageContents damageContents = new DamageContents()
            {
                DamageValue = totalDamage,
                CriticalRate = totalCritRate,
            };
            
            cachedDamage.ValueRW.Value = damageContents;
            
            ecb.RemoveComponent<ShouldSetDamageValuesComponent>(entity);
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public Entity GetWeaponEntity(ref SystemState state, WeaponType type)
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

    [BurstCompile]
    public DamageComponent GetDamageComponent(ref SystemState state, Entity entity)
    {
        var damageMod = state.EntityManager.GetComponentData<DamageComponent>(entity);
        return damageMod;
    }

    [BurstCompile]
    public DamageModifierComponent GetDamageModifierComponent(ref SystemState state, Entity entity)
    {
        var damageMod = state.EntityManager.GetComponentData<DamageModifierComponent>(entity);
        return damageMod;
    }
    
    [BurstCompile]
    public SkillModifierComponent GetSkillModifier(ref SystemState state, Entity entity, AttackType type)
    {
        var skillMod = state.EntityManager.GetComponentData<SkillModifierComponent>(entity);
        return skillMod;
    }
}

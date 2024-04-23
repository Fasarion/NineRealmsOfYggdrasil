using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

[UpdateAfter(typeof(UpgradeUISystem))]
public partial class UpgradeApplierSystem : SystemBase
{
    private UpgradePoolManager _pool;
    
    protected override void OnUpdate()
    {
        bool choiceExists = SystemAPI.TryGetSingletonRW(out RefRW<UpgradeChoice> choice);
        if (!choiceExists)
        {
            // No choice found";
            return;
        }
        
        if (choice.ValueRO.IsHandled) return;
        
            if (_pool == null)
            {
                _pool = UpgradePoolManager.Instance;
            }

            UpgradeObject upgradeObject = _pool.GetUpgradeObjectReferenceByKey(choice.ValueRO.ChoiceIndex);
            choice.ValueRW.IsHandled = true;
            
            HandleLocks(upgradeObject);
        
            Debug.Log($"Upgrade chosen: {upgradeObject.upgradeTitle}");

            Apply(upgradeObject);
    }

    public void Apply(UpgradeObject upgradeObject)
    {
        foreach (var upgrade in upgradeObject.upgrades)
        {
            UpgradeBaseType baseType = upgrade.thingToUpgrade;
            UpgradeValueTypes valueType = upgrade.valueToUpgrade;
            float upgradeAmount = upgrade.valueAmount;
            var entity = GetEntityToUpgrade(upgrade.thingToUpgrade);
            ApplyComponentUpgrade(valueType, upgradeAmount, entity);
        }

        InformStatHandler();
    }

    private void ApplyComponentUpgrade(UpgradeValueTypes upgradeValueToUpgrade, float valueAmount, Entity entity)
    {
        switch (upgradeValueToUpgrade)
        {
            case UpgradeValueTypes.damage:
                if (!EntityManager.HasComponent<DamageComponent>(entity))
                {
                    EntityManager.AddComponent<DamageComponent>(entity);
                }

                var damageComponent = EntityManager.GetComponentData<DamageComponent>(entity);
                damageComponent.Value.DamageValue += valueAmount;
                EntityManager.SetComponentData(entity, damageComponent);
                return;

            case UpgradeValueTypes.damageModifier:
                if (!EntityManager.HasComponent<DamageModifierComponent>(entity))
                {
                    EntityManager.AddComponent<DamageModifierComponent>(entity);
                }

                var damageModComponent = EntityManager.GetComponentData<DamageModifierComponent>(entity);
                damageModComponent.Value += valueAmount;
                EntityManager.SetComponentData(entity, damageModComponent);
                return;

            case UpgradeValueTypes.normalModifier:
                if (!EntityManager.HasComponent<SkillModifierComponent>(entity))
                {
                    EntityManager.AddComponent<SkillModifierComponent>(entity);
                }

                var normalMod = EntityManager.GetComponentData<SkillModifierComponent>(entity);
                normalMod.Value.Normal += valueAmount;
                EntityManager.SetComponentData(entity, normalMod);
                return;
            
            case UpgradeValueTypes.specialModifier:
                if (!EntityManager.HasComponent<SkillModifierComponent>(entity))
                {
                    EntityManager.AddComponent<SkillModifierComponent>(entity);
                }

                var specialMod = EntityManager.GetComponentData<SkillModifierComponent>(entity);
                specialMod.Value.Normal += valueAmount;
                EntityManager.SetComponentData(entity, specialMod);
                return;
            
            case UpgradeValueTypes.ultimateModifier:
                if (!EntityManager.HasComponent<SkillModifierComponent>(entity))
                {
                    EntityManager.AddComponent<SkillModifierComponent>(entity);
                }

                var ultMod = EntityManager.GetComponentData<SkillModifierComponent>(entity);
                ultMod.Value.Normal += valueAmount;
                EntityManager.SetComponentData(entity, ultMod);
                return;
            
            case UpgradeValueTypes.passiveModifier:
                if (!EntityManager.HasComponent<SkillModifierComponent>(entity))
                {
                    EntityManager.AddComponent<SkillModifierComponent>(entity);
                }

                var passiveMod = EntityManager.GetComponentData<SkillModifierComponent>(entity);
                passiveMod.Value.Normal += valueAmount;
                EntityManager.SetComponentData(entity, passiveMod);
                return;
        }
    }

    private Entity GetEntityToUpgrade(UpgradeBaseType upgradeThingToUpgrade)
    {
        switch (upgradeThingToUpgrade)
        {
                
            case UpgradeBaseType.Player:
                
                foreach (var(_, entity)  in SystemAPI.Query<PlayerTag>()
                    .WithEntityAccess())
                {
                    return entity;
                }
                break;
            
            
            case UpgradeBaseType.Sword:
                
                foreach (var(_, entity)  in SystemAPI.Query<SwordComponent>()
                             .WithEntityAccess())
                {
                    return entity;
                }
                break;
            case UpgradeBaseType.Hammer:
                
                foreach (var(_, entity)  in SystemAPI.Query<HammerComponent>()
                             .WithEntityAccess())
                {
                    return entity;
                }
                break;
                
        }

        return default;
    }

    private void InformStatHandler()
    {
        bool statHandlerExists = SystemAPI.TryGetSingletonRW(out RefRW<StatHandlerComponent> statHandler);
        bool attackCallerExists = SystemAPI.TryGetSingleton(out WeaponAttackCaller attackCaller);
        if (!statHandlerExists || !attackCallerExists)
        {
            Debug.LogWarning("Need Attack Caller and StatHandler to update weapon stats correctly.");
            return;
        }

        statHandler.ValueRW.ShouldUpdateStats = true;
        statHandler.ValueRW.WeaponType = attackCaller.ActiveAttackData.WeaponType;
        statHandler.ValueRW.AttackType = attackCaller.ActiveAttackData.AttackType;
        statHandler.ValueRW.ComboCounter = attackCaller.ActiveAttackData.Combo;
    }
    
    
    private void HandleLocks(UpgradeObject upgradeObject)
    {
        upgradeObject.isUnlocked = false;
        _pool.RegisterUpgradeAsPicked(upgradeObject.upgradeIndex);

        UpgradeObject[] objectsToUnlock = upgradeObject.upgradesToUnlock.ToArray();

        foreach (var obj in objectsToUnlock)
        {
            obj.isUnlocked = true;
        }
        
        UpgradeObject[] objectsToLock = upgradeObject.upgradesToLock.ToArray();

        foreach (var obj in objectsToLock)
        {
            obj.isUnlocked = false;
        }
    }
}

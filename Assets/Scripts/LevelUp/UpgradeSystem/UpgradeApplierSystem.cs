using System;
using System.Collections;
using System.Collections.Generic;
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
            var entity = GetEntityToUpgrade(upgrade.thingToUpgrade);
            // var component = GetComponentToUpgrade(upgrade.valueToUpgrade);
            //
            // ApplyUpgrade(upgrade, statsComponent);
        }

        InformStatHandler();
    }

    private object GetComponentToUpgrade(UpgradeValueTypes upgradeValueToUpgrade, Entity entity)
    {
        switch (upgradeValueToUpgrade)
        {
            case UpgradeValueTypes.damage:
                if (EntityManager.HasComponent<>())
                    return default;
                break;
        }

        return default;
    }

    private Entity GetEntityToUpgrade(UpgradeBaseType upgradeThingToUpgrade)
    {
        switch (upgradeThingToUpgrade)
        {
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

    private RefRW<CombatStatsComponent> GetStatsComponent(UpgradeBaseType baseType)
    {
        switch (baseType)
        {
            case UpgradeBaseType.Sword:
                
                foreach (var stats in SystemAPI.Query<RefRW<CombatStatsComponent>>()
                    .WithAll<SwordStatsTag>())
                {
                    return stats;
                }
                break;
            case UpgradeBaseType.Hammer:
                
                foreach (var stats in SystemAPI.Query<RefRW<CombatStatsComponent>>()
                             .WithAll<HammerStatsTag>())
                {
                    return stats;
                }
                break;
                
        }

        return default;
    }

    private void ApplyUpgrade(UpgradeInformation upgrade, RefRW<CombatStatsComponent> statsComponent)
    {
        switch (upgrade.valueToUpgrade)
        {
            case UpgradeValueTypes.damage:
                statsComponent.ValueRW.OverallStats.Damage.BaseValue += upgrade.valueAmount;
                break;
            case UpgradeValueTypes.cooldown:
                statsComponent.ValueRW.OverallStats.Cooldown.BaseValue -= upgrade.valueAmount;
                break;
            case UpgradeValueTypes.areaEffect:
                statsComponent.ValueRW.OverallStats.Area.BaseValue += upgrade.valueAmount;
                break;
            case UpgradeValueTypes.attackSpeed:
                statsComponent.ValueRW.OverallStats.AttackSpeed.BaseValue += upgrade.valueAmount;
                break;
            case UpgradeValueTypes.energyRegen:
                statsComponent.ValueRW.OverallStats.EnergyFillPerHit.BaseValue += upgrade.valueAmount;
                break;
        }
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

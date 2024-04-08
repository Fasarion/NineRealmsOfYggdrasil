using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(UpgradeUISystem))]
public partial class UpgradeApplierSystem : SystemBase
{
    private UpgradePoolManager _pool;
    
    protected override void OnUpdate()
    {
        bool choiseExists = SystemAPI.TryGetSingletonRW(out RefRW<UpgradeChoice> choice);
        if (choiseExists)
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
            var statsComponent = GetStatsComponent(baseType);
            
            ApplyUpgrade(upgrade, statsComponent);
        }
    }

    private RefRW<WeaponStatsComponent> GetStatsComponent(UpgradeBaseType baseType)
    {
        switch (baseType)
        {
            case UpgradeBaseType.Sword:
                
                foreach (var stats in SystemAPI.Query<RefRW<WeaponStatsComponent>>()
                    .WithAll<SwordStatsTag>())
                {
                    return stats;
                }
                break;
        }

        return default;
    }

    private void ApplyUpgrade(UpgradeInformation upgrade, RefRW<WeaponStatsComponent> statsComponent)
    {
        switch (upgrade.valueToUpgrade)
        {
            case UpgradeValueTypes.Damage:
                statsComponent.ValueRW.baseDamage += upgrade.valueAmount;
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

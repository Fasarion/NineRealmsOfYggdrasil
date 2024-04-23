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

                var component = EntityManager.GetComponentData<DamageComponent>(entity);
                component.Value.DamageValue += valueAmount;
                EntityManager.SetComponentData(entity, component);
                break;
                
                
        }
        
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

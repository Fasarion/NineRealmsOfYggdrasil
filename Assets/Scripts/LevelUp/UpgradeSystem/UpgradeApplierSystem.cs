using Damage;
using Movement;
using Patrik;
using Patrik.Special_Attack;
using Player;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

[UpdateAfter(typeof(UpgradeUISystem))]
public partial class UpgradeApplierSystem : SystemBase
{
    private UpgradePoolManager _pool;
    private int _weaponCount = 1;
    
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
            UpgradeValueTypes valueType = upgrade.valueToUpgrade;
            float upgradeAmount = upgrade.valueAmount;
            var entity = GetEntityToUpgrade(upgrade.thingToUpgrade);
            ApplyComponentUpgrade(valueType, upgradeAmount, entity);
        }

        InformStatHandler();
    }

    private WeaponType GetCurrentWeaponType(Entity entity)
    {
        WeaponType type;

        if (EntityManager.HasComponent<IceRingConfig>(entity))
        {
            type = WeaponType.Sword;
        }
        else if (EntityManager.HasComponent<HammerSpecialConfig>(entity))
        {
            type = WeaponType.Hammer;
        }
        else
        {
            type = WeaponType.Birds;
        }

        return type;
    }

    private void ApplyComponentUpgrade(UpgradeValueTypes upgradeValueToUpgrade, float valueAmount, Entity entity)
    {
        switch (upgradeValueToUpgrade)
        {
            case UpgradeValueTypes.UnlockSpecial:
            {
                EntityManager.AddComponent<IsUnlocked>(entity);
                var type = GetCurrentWeaponType(entity);
                EventManager.OnSpecialAttackUnlocked(type);
                return;
            }
            
            case UpgradeValueTypes.spawnCount:
                var spawnCountComponent = EntityManager.GetComponentData<SpawnCount>(entity);
                spawnCountComponent.Value += (int)valueAmount;
                EntityManager.SetComponentData(entity, spawnCountComponent);
                return;
            
            case UpgradeValueTypes.spawnCountMultiplier:
                var spawnCountMultiplierComponent = EntityManager.GetComponentData<SpawnCountMultiplier>(entity);
                spawnCountMultiplierComponent.Value += (int)valueAmount;
                EntityManager.SetComponentData(entity, spawnCountMultiplierComponent);
                return;
            
            case UpgradeValueTypes.Unlock:
                _weaponCount++;
                EventManager.OnWeaponCountSet?.Invoke(_weaponCount);
                return;
            
            case UpgradeValueTypes.damage:
                if (!EntityManager.HasComponent<DamageComponent>(entity))
                {
                    EntityManager.AddComponent<DamageComponent>(entity);
                }

                var damageComponent = EntityManager.GetComponentData<DamageComponent>(entity);
                damageComponent.Value.DamageValue += valueAmount;
                EntityManager.SetComponentData(entity, damageComponent);
                return;
            
            case UpgradeValueTypes.baseAtk:
                if (!EntityManager.HasComponent<DamageComponent>(entity))
                {
                    EntityManager.AddComponent<DamageComponent>(entity);
                }

                var damageComponent2 = EntityManager.GetComponentData<DamageComponent>(entity);
                damageComponent2.Value.DamageValue += valueAmount;
                EntityManager.SetComponentData(entity, damageComponent2);
                return;
            
            case UpgradeValueTypes.crit:
                if (!EntityManager.HasComponent<DamageComponent>(entity))
                {
                    EntityManager.AddComponent<DamageComponent>(entity);
                }

                var critComponent = EntityManager.GetComponentData<DamageComponent>(entity);
                critComponent.Value.CriticalRate += valueAmount;
                EntityManager.SetComponentData(entity, critComponent);
                return;
            
            case UpgradeValueTypes.attackSpeed:
                if (!EntityManager.HasComponent<AttackSpeedModifier>(entity))
                {
                    EntityManager.AddComponent<AttackSpeedModifier>(entity);
                }

                var attackSpeedComponent = EntityManager.GetComponentData<AttackSpeedModifier>(entity);
                attackSpeedComponent.Value += valueAmount;
                EntityManager.SetComponentData(entity, attackSpeedComponent);
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
            
            case UpgradeValueTypes.movementSpeed:
                if (!EntityManager.HasComponent<MoveSpeedComponent>(entity))
                {
                    EntityManager.AddComponent<MoveSpeedComponent>(entity);
                }

                var moveSpeed = EntityManager.GetComponentData<MoveSpeedComponent>(entity);
                moveSpeed.Value += valueAmount;
                EntityManager.SetComponentData(entity, moveSpeed);
                return;
            
            case UpgradeValueTypes.knockbackForce:
                if (!EntityManager.HasComponent<KnockBackOnHitComponent>(entity))
                {
                    EntityManager.AddComponent<KnockBackOnHitComponent>(entity);
                }

                var knockbackForce = EntityManager.GetComponentData<KnockBackOnHitComponent>(entity);
                knockbackForce.Value += valueAmount;
                EntityManager.SetComponentData(entity, knockbackForce);
                return;
            
            case UpgradeValueTypes.hitStopDuration:
                if (!EntityManager.HasComponent<ShouldApplyHitStopOnHit>(entity))
                {
                    EntityManager.AddComponent<ShouldApplyHitStopOnHit>(entity);
                }

                var hitStopDuration = EntityManager.GetComponentData<ShouldApplyHitStopOnHit>(entity);
                hitStopDuration.Duration += valueAmount;
                EntityManager.SetComponentData(entity, hitStopDuration);
                return;
            
            case UpgradeValueTypes.applyFire:
                if (!EntityManager.HasComponent<ElementalShouldApplyFireComponent>(entity))
                {
                    EntityManager.AddComponent<ElementalShouldApplyFireComponent>(entity);
                }

                return;
            
            case UpgradeValueTypes.applyLightning:
                if (!EntityManager.HasComponent<ElementalShouldApplyLightningComponent>(entity))
                {
                    EntityManager.AddComponent<ElementalShouldApplyLightningComponent>(entity);
                }

                return;
            
            case UpgradeValueTypes.applyIce:
                if (!EntityManager.HasComponent<ElementalShouldApplyIceComponent>(entity))
                {
                    EntityManager.AddComponent<ElementalShouldApplyIceComponent>(entity);
                }

                return;
            
            case UpgradeValueTypes.UseMousePosition:
                EntityManager.AddComponent<UseMousePosition>(entity);
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
            
            case UpgradeBaseType.Birds:
                
                foreach (var(_, entity)  in SystemAPI.Query<BirdsComponent>()
                    .WithEntityAccess())
                {
                    return entity;
                }
                break;
            
            case UpgradeBaseType.BirdSpecialAbility:
                
                foreach (var(_, entity)  in SystemAPI.Query<BirdsSpecialAttackConfig>()
                             .WithEntityAccess())
                {
                    return entity;
                }
                break;
                
            case UpgradeBaseType.SwordSpecialAbility:
                
                foreach (var(_, entity)  in SystemAPI.Query<IceRingConfig>()
                             .WithEntityAccess())
                {
                    return entity;
                }
                break;
            
            case UpgradeBaseType.HammerSpecialAbility:
                foreach (var(_, entity)  in SystemAPI.Query<HammerSpecialConfig>()
                             .WithEntityAccess())
                {
                    return entity;
                }
                break;
            
            case UpgradeBaseType.HammerUltimateAbility:
                
                foreach (var(_, entity)  in SystemAPI.Query<ThunderStrikeConfig>()
                             .WithEntityAccess())
                {
                    return entity;
                }
                break;
            
            case UpgradeBaseType.SwordUltimateAbility:
                
                foreach (var(_, entity)  in SystemAPI.Query<SwordUltimateConfig>()
                             .WithEntityAccess())
                {
                    return entity;
                }
                break;
            
            case UpgradeBaseType.BirdUltimateAbility:
                
                foreach (var(_, entity)  in SystemAPI.Query<BirdsUltimateAttackConfig>()
                             .WithEntityAccess())
                {
                    return entity;
                }
                break;
            
            case UpgradeBaseType.SwordComboAbility:
                
                foreach (var(_, entity)  in SystemAPI.Query<SwordComboAbilityConfig>()
                    .WithEntityAccess())
                {
                    return entity;
                }
                break;
            
            case UpgradeBaseType.HammerComboAbility:
                
                foreach (var(_, entity)  in SystemAPI.Query<ThunderBoltConfig>()
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

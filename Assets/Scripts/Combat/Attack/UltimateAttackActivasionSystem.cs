using Destruction;
using Patrik;
using Unity.Burst;
using Unity.Entities;
using Unity.Entities.Graphics;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

[UpdateAfter(typeof(UpdateMouseWorldPositionSystem))]
[UpdateBefore(typeof(PlayerAttackSystem))]
public partial struct UltimateAttackActivasionSystem : ISystem
{
    private bool hasPreparedUltimate;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTargetingPrefab>();
        
        state.RequireForUpdate<MousePositionInput>();
        state.RequireForUpdate<PrimaryButtonInput>();
        state.RequireForUpdate<PlayerNormalAttackInput>();
        state.RequireForUpdate<PlayerSpecialAttackInput>();
        
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<GameManagerSingleton>();
        state.RequireForUpdate<PlayerTargetInfoSingleton>();
        
        state.RequireForUpdate<GameUnpaused>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        HandleActivation(ref state);
    }
    


    [BurstCompile]
    public void HandleActivation(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        
        bool targetExists = SystemAPI.TryGetSingletonEntity<PlayerTargetingComponent>(out Entity targeter);
        
        attackCaller.ValueRW.PrepareUltimateInfo.Perform = false;
        attackCaller.ValueRW.PrepareUltimateInfo.HasPreparedThisFrame = false;
        attackCaller.ValueRW.PrepareUltimateInfo.IsPreparing = false;
        
        bool isBusy = attackCaller.ValueRO.BusyAttackInfo.IsBusy(AttackType.Ultimate,
            attackCaller.ValueRO.ActiveAttackData.WeaponType);

        var gameManager = SystemAPI.GetSingletonRW<GameManagerSingleton>();
        gameManager.ValueRW.CombatState = hasPreparedUltimate ? CombatState.ActivatingUltimate : CombatState.Normal;
        
        bool activeWeaponHasFullEnergy = false;
        Entity weaponEntity = Entity.Null;
        bool weaponUsesTargeting = false;

        // fetch weapon info
        foreach (var (weapon, energyBar, entity) in SystemAPI
            .Query<RefRW<WeaponComponent>, RefRW<EnergyBarComponent>>()
            .WithAll<ActiveWeapon>()
            .WithNone<ResetEnergyTag>()
            .WithEntityAccess())
        {
            activeWeaponHasFullEnergy = energyBar.ValueRO.IsFull;
            weaponEntity = entity;
            weaponUsesTargeting = weapon.ValueRO.UsesTargetingForUltimate;
        }

        // return if weapon doesn't have enough energy
        if (!activeWeaponHasFullEnergy)
        {
            // removing existing target 
          //  var targetExists = SystemAPI.TryGetSingletonEntity<PlayerTargetingComponent>(out Entity targeter);
            if (targetExists)
            {
                state.EntityManager.AddComponent<ShouldBeDestroyed>(targeter);
            }

            hasPreparedUltimate = false;
            return;
        }

        // handle ultimate weapon that doesn't use targeting
        var ultimateAttackKeyPressed = SystemAPI.GetSingleton<PlayerUltimateAttackInput>().KeyDown;
        bool isPreparingAttack = attackCaller.ValueRO.IsPreparingAttack();
        
        if (!weaponUsesTargeting)
        {
            if (ultimateAttackKeyPressed && !isPreparingAttack && !isBusy && UltUnlocked(ref state, attackCaller.ValueRO.ActiveAttackData.WeaponType))
            {
                state.EntityManager.SetComponentEnabled<ResetEnergyTag>(weaponEntity, true);
                attackCaller.ValueRW.PrepareUltimateInfo.Perform = true;
            }
            
            // removing existing target 
          //  var targetExists = SystemAPI.TryGetSingletonEntity<PlayerTargetingComponent>(out Entity targeter);
            if (targetExists)
            {
                state.EntityManager.AddComponent<ShouldBeDestroyed>(targeter);
            }
            
            hasPreparedUltimate = false;
            return;
        }
        
        // Instantiate target
        if (ultimateAttackKeyPressed && !hasPreparedUltimate && !isPreparingAttack && !isBusy 
            && UltUnlocked(ref state, attackCaller.ValueRO.ActiveAttackData.WeaponType))
        {
            var playerTargetPrefab = SystemAPI.GetSingleton<PlayerTargetingPrefab>();
            state.EntityManager.Instantiate(playerTargetPrefab.Value);
            hasPreparedUltimate = true;
            attackCaller.ValueRW.PrepareUltimateInfo.HasPreparedThisFrame = true;
            return;
        }
        
        // handle following
        var mousePos = SystemAPI.GetSingleton<MousePositionInput>();
        var targetSingleton = SystemAPI.GetSingletonRW<PlayerTargetInfoSingleton>();
        foreach (var (transform, target) in SystemAPI.Query<RefRW<LocalTransform>, PlayerTargetingComponent>())
        {
            transform.ValueRW.Position = mousePos.WorldPosition;
            targetSingleton.ValueRW.LastPosition = transform.ValueRW.Position; 
        }

        // handle activating ultimate
        bool activationKeyPressed = SystemAPI.GetSingleton<PlayerNormalAttackInput>().KeyDown 
                                    || SystemAPI.GetSingleton<PlayerSpecialAttackInput>().KeyDown
                                    || SystemAPI.GetSingleton<PlayerUltimateAttackInput>().KeyDown;
        
        if (activationKeyPressed && hasPreparedUltimate && !isBusy)
        {
            state.EntityManager.SetComponentEnabled<ResetEnergyTag>(weaponEntity, true);
            attackCaller.ValueRW.PrepareUltimateInfo.Perform = true;
            
           // var targetExists = SystemAPI.TryGetSingletonEntity<PlayerTargetingComponent>(out Entity targeter);
            if (targetExists)
            {
                state.EntityManager.AddComponent<ShouldBeDestroyed>(targeter);
            }

            hasPreparedUltimate = false;
        }

        attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        attackCaller.ValueRW.PrepareUltimateInfo.IsPreparing = hasPreparedUltimate;
    }

    private bool UltUnlocked(ref SystemState state, WeaponType weaponType)
    {
        bool entityExists;
        bool lookupUnlock;
        
        switch (weaponType)
        {
            case WeaponType.Sword:
                 entityExists = SystemAPI.TryGetSingletonEntity<SwordUltimateConfig>(out Entity swordSpecial);
                 lookupUnlock = entityExists && SystemAPI.HasComponent<IsUnlocked>(swordSpecial);

                return lookupUnlock;
            
            case WeaponType.Hammer:
                entityExists = SystemAPI.TryGetSingletonEntity<ThunderStrikeConfig>(out Entity hammer);
                lookupUnlock = entityExists && SystemAPI.HasComponent<IsUnlocked>(hammer);
                
                return lookupUnlock;

            case WeaponType.Birds:
                entityExists = SystemAPI.TryGetSingletonEntity<BirdsUltimateAttackConfig>(out Entity birds);
                lookupUnlock = entityExists && SystemAPI.HasComponent<IsUnlocked>(birds);
                
                return lookupUnlock;
        }

        return false;
    }
    
    // private bool CheckForUnlock<T>(ref SystemState state) where T : unmanaged, IComponentData
    // {
    //     bool entityExists = SystemAPI.TryGetSingletonEntity<T>(out Entity swordSpecial);
    //     bool lookupUnlock = entityExists && SystemAPI.HasComponent<IsUnlocked>(swordSpecial);
    //
    //     return lookupUnlock;
    // }
}
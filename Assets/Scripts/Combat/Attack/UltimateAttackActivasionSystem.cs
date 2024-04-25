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
        
        attackCaller.ValueRW.PrepareUltimateInfo.Perform = false;
        attackCaller.ValueRW.PrepareUltimateInfo.HasPreparedThisFrame = false;
        attackCaller.ValueRW.PrepareUltimateInfo.IsPreparing = false;

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
            var targetExists = SystemAPI.TryGetSingletonEntity<PlayerTargetingComponent>(out Entity targeter);
            if (targetExists)
            {
                state.EntityManager.AddComponent<ShouldBeDestroyed>(targeter);
            }

            hasPreparedUltimate = false;
            return;
        }

        // handle ultimate weapon that doesn't use targeting
        var ultimateAttackKeyPressed = SystemAPI.GetSingleton<PlayerUltimateAttackInput>().KeyPressed;
        bool isPreparingAttack = attackCaller.ValueRO.IsPreparingAttack();
        
        if (!weaponUsesTargeting)
        {
            if (ultimateAttackKeyPressed && !isPreparingAttack)
            {
                state.EntityManager.SetComponentEnabled<ResetEnergyTag>(weaponEntity, true);
                attackCaller.ValueRW.PrepareUltimateInfo.Perform = true;
            }
            
            // removing existing target 
            var targetExists = SystemAPI.TryGetSingletonEntity<PlayerTargetingComponent>(out Entity targeter);
            if (targetExists)
            {
                state.EntityManager.AddComponent<ShouldBeDestroyed>(targeter);
            }
            
            hasPreparedUltimate = false;
            return;
        }
        
        // Instantiate target
        if (ultimateAttackKeyPressed && !hasPreparedUltimate && !isPreparingAttack)
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
        bool activationKeyPressed = SystemAPI.GetSingleton<PlayerNormalAttackInput>().KeyPressed 
                                    || SystemAPI.GetSingleton<PlayerSpecialAttackInput>().KeyDown
                                    || SystemAPI.GetSingleton<PlayerUltimateAttackInput>().KeyPressed;
        //bool ultKeyPressedAfterActivation = SystemAPI.GetSingleton<PlayerUltimateAttackInput>().KeyPressed && 
        
        if (activationKeyPressed && hasPreparedUltimate)
        {
            state.EntityManager.SetComponentEnabled<ResetEnergyTag>(weaponEntity, true);
            attackCaller.ValueRW.PrepareUltimateInfo.Perform = true;
            
            var targetExists = SystemAPI.TryGetSingletonEntity<PlayerTargetingComponent>(out Entity targeter);
            if (targetExists)
            {
                state.EntityManager.AddComponent<ShouldBeDestroyed>(targeter);
            }

            hasPreparedUltimate = false;
        }

        attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        attackCaller.ValueRW.PrepareUltimateInfo.IsPreparing = hasPreparedUltimate;
    }
}
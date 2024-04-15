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
        state.RequireForUpdate<PerformUltimateAttack>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var performUltra = SystemAPI.GetSingletonRW<PerformUltimateAttack>();
        performUltra.ValueRW.Value = false;
        
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
            var targetExists = SystemAPI.TryGetSingletonEntity<PlayerTargettingComponent>(out Entity targeter);
            if (targetExists)
            {
                state.EntityManager.AddComponent<ShouldBeDestroyed>(targeter);
            }

            hasPreparedUltimate = false;
            return;
        }

        // handle ultimate weapon that doesn't use targeting
        var ultimateAttackKeyPressed = SystemAPI.GetSingleton<PlayerUltimateAttackInput>().KeyPressed;
        if (!weaponUsesTargeting)
        {
            if (ultimateAttackKeyPressed)
            {
                state.EntityManager.SetComponentEnabled<ResetEnergyTag>(weaponEntity, true);
                performUltra.ValueRW.Value = true;
            }
            
            // removing existing target 
            var targetExists = SystemAPI.TryGetSingletonEntity<PlayerTargettingComponent>(out Entity targeter);
            if (targetExists)
            {
                state.EntityManager.AddComponent<ShouldBeDestroyed>(targeter);
            }

            hasPreparedUltimate = false;

            return;
        }
        
        // handle  weapon that does use targeting
        if (ultimateAttackKeyPressed && !hasPreparedUltimate)
        {
            var playerTargetPrefab = SystemAPI.GetSingleton<PlayerTargetingPrefab>();
            state.EntityManager.Instantiate(playerTargetPrefab.Value);
            hasPreparedUltimate = true;
            return;
        }
        
        // handle following
        var mousePos = SystemAPI.GetSingleton<MousePositionInput>();
        foreach (var (transform, target) in SystemAPI.Query<RefRW<LocalTransform>, PlayerTargettingComponent>())
        {
            transform.ValueRW.Position = mousePos.WorldPosition;
        }

        // handle activating ultimate
        bool activationKeyPressed = SystemAPI.GetSingleton<PlayerNormalAttackInput>().KeyPressed;
        if (activationKeyPressed)
        {
            state.EntityManager.SetComponentEnabled<ResetEnergyTag>(weaponEntity, true);
            performUltra.ValueRW.Value = true;
            
            var targetExists = SystemAPI.TryGetSingletonEntity<PlayerTargettingComponent>(out Entity targeter);
            if (targetExists)
            {
                state.EntityManager.AddComponent<ShouldBeDestroyed>(targeter);
            }

            hasPreparedUltimate = false;
        }
    }
}
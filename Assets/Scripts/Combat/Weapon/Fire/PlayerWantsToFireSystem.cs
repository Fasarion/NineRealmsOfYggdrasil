using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.Entities;
using UnityEngine;

namespace Weapon
{
    
[UpdateAfter(typeof(UpdateWeaponCooldownSystem))]
public partial struct PlayerWantsToFireSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FireSettingsData>();
        state.RequireForUpdate<PlayerWeaponConfig>();
        state.RequireForUpdate<PlayerFireInput>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        var fireSettings = SystemAPI.GetSingletonRW<FireSettingsData>();

        if (fireSettings.ValueRO.autoFire)
        {
            HandleAutoFire(ref state);
        }
        else
        {
            HandleManualFire(ref state);
        }
    }

    private void HandleAutoFire(ref SystemState state)
    {
        FireWithWeapons(ref state);
    }

    private void HandleManualFire(ref SystemState state)
    {
        bool fireButtonPressed = SystemAPI.GetSingleton<PlayerFireInput>().FireKeyPressed;
        if (!fireButtonPressed) return;

        FireWithWeapons(ref state);
    }

    private void FireWithWeapons(ref SystemState state)
    {
        foreach (var weapon in SystemAPI.Query<RefRW<WeaponComponent>>().WithAll<BelongsToPlayerTag>())
        {
            if (weapon.ValueRO.HasCooledDown)
            {
                Fire(weapon);
            }
        }
    }
    
    private void Fire(RefRW<WeaponComponent> weapon)
    {
        weapon.ValueRW.WantsToFire = true;
        ResetCoolDown(weapon);
    }

    private void ResetCoolDown(RefRW<WeaponComponent> weapon)
    {
        weapon.ValueRW.CurrentCoolDownTime = 0;
    }
}
}

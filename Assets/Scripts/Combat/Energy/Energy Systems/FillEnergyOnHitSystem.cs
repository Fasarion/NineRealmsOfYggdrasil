using Damage;
using Health;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateBefore(typeof(HandleHitBufferSystem))]
[UpdateInGroup(typeof(CombatSystemGroup))]
//[UpdateAfter(typeof(DetectHitTriggerSystem))]
public partial struct FillEnergyOnHitSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        //state.RequireForUpdate<EnergyBarComponent>();
        //state.RequireForUpdate<EnergyFillComponent>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        ResetHasChangedEnergy(ref state);
        HandleEnergyFill(ref state);
    }
    
    private void ResetHasChangedEnergy(ref SystemState state)
    {
        // Reset Has Changed Energy
        foreach (var (_, entity) in SystemAPI.Query<HasChangedEnergy>().WithEntityAccess())
        {
            state.EntityManager.SetComponentEnabled<HasChangedEnergy>(entity, false);
        }
    }


    private void HandleEnergyFill(ref SystemState state)
    {
        // direct hit
     //   FillEnergyFromActiveHits(ref state);
        FillEnergyFromPassiveHits(ref state);
        
        // projectile hits
        FillEnergyFromProjectiles(ref state);
    }

    private void FillEnergyFromActiveHits(ref SystemState state)
    {
        // go through all active weapons (should be 1 maximum)
        foreach (var (energyFill, hitBuffer) in SystemAPI
                .Query<EnergyFillComponent, DynamicBuffer<HitBufferElement>>()
                .WithAll<WeaponComponent, ActiveWeapon>())
        {
            float energyToApply = 0; 

            foreach (var hit in hitBuffer)
            {
                if (hit.IsHandled) continue;

                energyToApply += energyFill.ActiveFillPerHit;
            }
            
            // exit early if no energy to apply
            if (energyToApply == 0)
                continue;

            // go through all passive weapons to fill their bars
            foreach (var ( barToFill, weaponComponent) in SystemAPI
                    .Query<RefRW<EnergyBarComponent>, WeaponComponent>()
                    .WithNone<ActiveWeapon>())
            {
                // exit out if energy bar is already fill
                if (barToFill.ValueRO.CurrentEnergy >= barToFill.ValueRO.MaxEnergy)
                {
                    continue;
                }
            }
        }
    }

    private void FillEnergyFromPassiveHits(ref SystemState state)
    {
        // Fill passive energy bars - direct hit
        foreach (var (energyFill, energyBar, hitBuffer, weaponComponent) in
            SystemAPI.Query<EnergyFillComponent, RefRW<EnergyBarComponent>, DynamicBuffer<HitBufferElement>, WeaponComponent>()
                .WithNone<ActiveWeapon>())
        {
            // exit out if energy bar is already fill
            if (energyBar.ValueRO.IsFull)
            {
                continue;
            }

            foreach (var hit in hitBuffer)
            {
                if (hit.IsHandled) continue;

                float newEnergy = energyBar.ValueRO.CurrentEnergy + energyFill.PassiveFillPerHit;
                energyBar.ValueRW.CurrentEnergy = newEnergy;

                if (energyBar.ValueRO.IsFull)
                {
                    // cap energy to max
                    energyBar.ValueRW.CurrentEnergy = energyBar.ValueRO.MaxEnergy;
                    break;
                }
            }
        }
    }

   
    private void FillEnergyFromProjectiles(ref SystemState state)
    {
        // Fill passive energy bar from projectiles
        // TODO: make job
        foreach (var (ownerWeapon, projectileHitBuffer) in
            SystemAPI.Query<OwnerWeapon, DynamicBuffer<HitBufferElement>>().WithAll<HasChangedHP>())
        {
            Entity ownerEntity = ownerWeapon.Value;
            
            bool ownerIsActive = ownerWeapon.OwnerWasActive;
            if (ownerIsActive)
            {
                continue;
            }

            EnergyFillComponent energyFill = state.EntityManager.GetComponentData<EnergyFillComponent>(ownerEntity);
            EnergyBarComponent energyBar = state.EntityManager.GetComponentData<EnergyBarComponent>(ownerEntity);

            // exit early if the bar already has reached its max energy
            if (energyBar.IsFull)
            {
                continue;
            }

            float totalEnergyChange = 0;
            foreach (var hit in projectileHitBuffer)
            {
                if (hit.IsHandled)
                {
                    continue;
                }

                totalEnergyChange += energyFill.PassiveFillPerHit;
            }

            // energy has changed
            if (totalEnergyChange > 0)
            {
                state.EntityManager.SetComponentEnabled<HasChangedEnergy>(ownerEntity, true);
                float previousEnergyChange = state.EntityManager.GetComponentData<HasChangedEnergy>(ownerEntity).Value;
                state.EntityManager.SetComponentData(ownerEntity,
                    new HasChangedEnergy {Value = totalEnergyChange + previousEnergyChange});
            }
            else
            {
                continue;
            }

            float newEnergy = energyBar.CurrentEnergy + totalEnergyChange;
            if (newEnergy >= energyBar.MaxEnergy)
            {
                newEnergy = energyBar.MaxEnergy;
                // maybe todo: here we find where the energy reached max, inform UI
            }

            EnergyBarComponent newEnergyBar = new EnergyBarComponent
            {
                CurrentEnergy = newEnergy,
                MaxEnergy = energyBar.MaxEnergy
            };

            state.EntityManager.SetComponentData(ownerEntity, newEnergyBar);
        }
    }
}

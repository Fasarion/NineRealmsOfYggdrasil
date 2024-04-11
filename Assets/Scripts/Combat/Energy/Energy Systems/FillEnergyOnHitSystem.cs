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
        
        // TODO: Fill active energy bars

        FillEnergyFromProjectiles(ref state);


        // Fill passive energy bars - direct hit
        foreach (var (energyFill, energyBar, hitBuffer, weaponComponent) in 
            SystemAPI.Query<EnergyFillComponent, RefRW<EnergyBarComponent>, DynamicBuffer<HitBufferElement>, WeaponComponent>()
                .WithNone<ActiveWeapon>())
        {
            // exit out if energy bar is already fill
            if (energyBar.ValueRO.CurrentEnergy >= energyBar.ValueRO.MaxEnergy)
            {
                  continue;  
            }

            foreach (var hit in hitBuffer)
            {
                if (hit.IsHandled) continue;

                float newEnergy = energyBar.ValueRO.CurrentEnergy + energyFill.PassiveFillPerHit;
                energyBar.ValueRW.CurrentEnergy = newEnergy;
                
              //  Debug.Log($"New energy: {energyBar.ValueRO.CurrentEnergy}");
                
                if (energyBar.ValueRO.CurrentEnergy >= energyBar.ValueRO.MaxEnergy)
                {
                    // cap energy to max
                    energyBar.ValueRW.CurrentEnergy = energyBar.ValueRO.MaxEnergy;
                    break;
                }
            }
            
            
            // Handle active bars
        }
    }

    private void ResetHasChangedEnergy(ref SystemState state)
    {
        // Reset Has Changed Energy
        foreach (var (_, entity) in SystemAPI.Query<HasChangedEnergy>().WithEntityAccess())
        {
            state.EntityManager.SetComponentEnabled<HasChangedEnergy>(entity, false);
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

            // Ignore projectiles from active weapons
            
            bool ownerIsActive = state.EntityManager.IsComponentEnabled(ownerEntity, typeof(ActiveWeapon));
            if (ownerIsActive)
            {
                continue;
            }

            EnergyFillComponent energyFill = state.EntityManager.GetComponentData<EnergyFillComponent>(ownerEntity);
            EnergyBarComponent energyBar = state.EntityManager.GetComponentData<EnergyBarComponent>(ownerEntity);

            // exit early if the bar already has reached its max energy
            if (energyBar.CurrentEnergy >= energyBar.MaxEnergy)
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

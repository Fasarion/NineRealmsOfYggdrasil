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
      //  ResetHasChangedEnergy(ref state);
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
        FillEnergyFromActiveHits(ref state);
        FillEnergyFromPassiveHits(ref state);
        
        // projectile hits
        FillEnergyFromProjectiles(ref state);
        // TODO: Active projectiles
    }

    private void FillEnergyFromActiveHits(ref SystemState state)
    {
        // go through all active weapons (should be 1 maximum)
        foreach (var (energyFill, hitBuffer) in SystemAPI
                .Query<EnergyFillComponent, DynamicBuffer<HitBufferElement>>()
                .WithAll<WeaponComponent, ActiveWeapon>())
        {
            int hitCount = 0;
            
            foreach (var hit in hitBuffer)
            {
                if (hit.IsHandled) continue;

                hitCount++;
            }
            
            // exit early if no hits
            if (hitCount == 0)
                continue;

            float totalEnergyFill = hitCount * energyFill.ActiveFillPerHit;

            // go through all passive weapons to fill their bars
            foreach (var ( barToFill, weaponComponent, passiveEntity) in SystemAPI
                    .Query<RefRW<EnergyBarComponent>, WeaponComponent>()
                    .WithNone<ActiveWeapon>()
                    .WithEntityAccess())
            {
                // exit out if energy bar is already full
                if (barToFill.ValueRO.IsFull)
                {
                    continue;
                }

                float oldEnergy = barToFill.ValueRO.CurrentEnergy;

                float newEnergy = oldEnergy + totalEnergyFill;
                if (newEnergy >= barToFill.ValueRO.MaxEnergy)
                {
                    newEnergy = barToFill.ValueRO.MaxEnergy;
                }

                barToFill.ValueRW.CurrentEnergy = newEnergy;
                float deltaEnergy = newEnergy - oldEnergy; 
                OnEnergyChange(ref state, passiveEntity, deltaEnergy);
            }
        }
    }

    private void FillEnergyFromPassiveHits(ref SystemState state)
    {
        // Fill passive energy bars - direct hit
        foreach (var (energyFill, energyBar, hitBuffer, weaponComponent, entity) in
            SystemAPI.Query<EnergyFillComponent, RefRW<EnergyBarComponent>, DynamicBuffer<HitBufferElement>, WeaponComponent>()
                .WithNone<ActiveWeapon>()
                .WithEntityAccess())
        {
            // exit out if energy bar is already fill
            if (energyBar.ValueRO.IsFull)
            {
                continue;
            }
            
            int hitCount = 0;
            
            foreach (var hit in hitBuffer)
            {
                if (hit.IsHandled) continue;

                hitCount++;
            }
            
            // exit early if no hits
            if (hitCount == 0)
                continue;

            float totalEnergyFill = hitCount * energyFill.ActiveFillPerHit;
            
            float oldEnergy = energyBar.ValueRO.CurrentEnergy;

            float newEnergy = oldEnergy + totalEnergyFill;
            if (newEnergy >= energyBar.ValueRO.MaxEnergy)
            {
                newEnergy = energyBar.ValueRO.MaxEnergy;
            }

            energyBar.ValueRW.CurrentEnergy = newEnergy;
            float deltaEnergy = newEnergy - oldEnergy; 
            OnEnergyChange(ref state, entity, deltaEnergy);
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
                OnEnergyChange(ref state, ownerEntity, totalEnergyChange);
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

    private static void OnEnergyChange(ref SystemState state, Entity entity, float changeValue)
    {
        state.EntityManager.SetComponentEnabled<HasChangedEnergy>(entity, true);
        state.EntityManager.SetComponentData(entity, new HasChangedEnergy {Value = changeValue});
    }
}

using Damage;
using Health;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateBefore(typeof(HandleHitBufferSystem))]
[UpdateInGroup(typeof(CombatSystemGroup))]
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
        HandleEnergyReset(ref state);
        HandleEnergyFill(ref state);
    }
    
    private void ResetHasChangedEnergy(ref SystemState state)
    {
        foreach (var (_, entity) in SystemAPI.Query<HasChangedEnergy>().WithEntityAccess())
        {
            state.EntityManager.SetComponentEnabled<HasChangedEnergy>(entity, false);
        }
    }
    
    private void HandleEnergyReset(ref SystemState state)
    {
        foreach (var ( bar, entity) in SystemAPI
            .Query<RefRW<EnergyBarComponent>>()
            .WithAll<ResetEnergyTag>()
            .WithEntityAccess())
        {
            state.EntityManager.SetComponentEnabled<ResetEnergyTag>(entity, false);
            float energyLoss = -bar.ValueRO.MaxEnergy;
            
            Debug.Log($"Add {energyLoss} energy");
            
            FillEnergyBarWithRef(ref state, bar, energyLoss, entity);
        }
    }

    private void HandleEnergyFill(ref SystemState state)
    {
        // direct hit
        FillEnergyFromDirectActiveHits(ref state);
        FillEnergyFromDirectPassiveHits(ref state);
        
        // projectile hits
        FillEnergyFromPassiveProjectiles(ref state);
        // TODO: Active projectiles
    }

    private void FillEnergyFromDirectActiveHits(ref SystemState state)
    {
        // go through all active weapons (should be 1 maximum)
        foreach (var (energyFill, hitBuffer) in SystemAPI
                .Query<EnergyFillComponent, DynamicBuffer<HitBufferElement>>()
                .WithAll<WeaponComponent, ActiveWeapon>())
        {
            
            if (!HasHit(hitBuffer, out var hitCount))
                continue;
            
            float totalEnergyFill = hitCount * energyFill.ActiveFillPerHit;

            // go through all passive weapons to fill their bars
            foreach (var ( barToFill, weaponComponent, passiveEntity) in SystemAPI
                    .Query<RefRW<EnergyBarComponent>, WeaponComponent>()
                    .WithNone<ActiveWeapon>()
                    .WithEntityAccess())
            {
                // exit out if energy bar is already full
                if (barToFill.ValueRO.IsFull) continue;
                
                FillEnergyBarWithRef(ref state, barToFill, totalEnergyFill, passiveEntity);
            }
        }
    }

    private void FillEnergyFromDirectPassiveHits(ref SystemState state)
    {
        // Fill passive energy bars - direct hit
        foreach (var (energyFill, energyBar, weapon, hitBuffer, entity) in
            SystemAPI.Query<EnergyFillComponent, RefRW<EnergyBarComponent>, WeaponComponent, DynamicBuffer<HitBufferElement>>()
                .WithNone<ActiveWeapon>()
                .WithEntityAccess())
        {
            // exit out if energy bar is already fill
            if (energyBar.ValueRO.IsFull) continue;

            // exit if hit buffer has not hit
            if (!HasHit(hitBuffer, out int hitCount)) continue;
            
            float totalEnergyFill = hitCount * energyFill.ActiveFillPerHit;
            FillEnergyBarWithRef(ref state, energyBar, totalEnergyFill, entity);
        }
    }
    
    private void FillEnergyFromPassiveProjectiles(ref SystemState state)
    {
        foreach (var (ownerWeapon, projectileHitBuffer) in
            SystemAPI.Query<OwnerWeapon, DynamicBuffer<HitBufferElement>>().WithAll<HasChangedHP>())
        {
            // only fill when owner is passive
            if (ownerWeapon.OwnerWasActive) continue;
            
            Entity ownerEntity = ownerWeapon.Value;
            EnergyBarComponent energyBar = state.EntityManager.GetComponentData<EnergyBarComponent>(ownerEntity);

            // exit early if the bar already has reached its max energy
            if (energyBar.IsFull) continue;

            if (!HasHit(projectileHitBuffer, out int hitCount)) continue;

            EnergyFillComponent energyFill = state.EntityManager.GetComponentData<EnergyFillComponent>(ownerEntity);
            float totalEnergyChange = hitCount * energyFill.PassiveFillPerHit;
            FillEnergyBarWithEM(ref state, energyBar, totalEnergyChange, ownerEntity);
        }
    }

    private bool HasHit(DynamicBuffer<HitBufferElement> hitBuffer, out int hitCount)
    {
        hitCount = GetHitCount(hitBuffer);
        return hitCount > 0;
    }

    private int GetHitCount(DynamicBuffer<HitBufferElement> hitBuffer)
    {
        int hitCount = 0;

        foreach (var hit in hitBuffer)
        {
            if (!hit.IsHandled)
                hitCount++;
        }

        return hitCount;
    }

    private static void FillEnergyBarWithRef(ref SystemState state, RefRW<EnergyBarComponent> energyBar, float energyFill, Entity entity)
    {
        var oldEnergy = energyBar.ValueRO.CurrentEnergy;
        var newEnergy = GetNewEnergy(energyFill, oldEnergy, energyBar.ValueRO.MaxEnergy);
        float deltaEnergy = newEnergy - oldEnergy;

        energyBar.ValueRW.CurrentEnergy = newEnergy;
        
        UpdateHasChangedEnergy(ref state, entity, deltaEnergy);
    }
    
    private static void FillEnergyBarWithEM(ref SystemState state, EnergyBarComponent energyBar, float energyFill, Entity entity)
    {
        float oldEnergy = energyBar.CurrentEnergy;
        float newEnergy = GetNewEnergy(energyFill, oldEnergy, energyBar.MaxEnergy);
        float deltaEnergy = newEnergy - oldEnergy;

        EnergyBarComponent newEnergyBar = new EnergyBarComponent
        {
            CurrentEnergy = newEnergy,
            MaxEnergy = energyBar.MaxEnergy
        };
        state.EntityManager.SetComponentData(entity, newEnergyBar);
        
        UpdateHasChangedEnergy(ref state, entity, deltaEnergy);
    }

    private static float GetNewEnergy(float energyFill, float currentEnergy, float maxEnergy)
    {
        float newEnergy = currentEnergy + energyFill;
        if (newEnergy >= maxEnergy)
            newEnergy = maxEnergy;
        return newEnergy;
    }

    private static void UpdateHasChangedEnergy(ref SystemState state, Entity entity, float changeValue)
    {
        state.EntityManager.SetComponentEnabled<HasChangedEnergy>(entity, true);
        state.EntityManager.SetComponentData(entity, new HasChangedEnergy {Value = changeValue});
    }
}
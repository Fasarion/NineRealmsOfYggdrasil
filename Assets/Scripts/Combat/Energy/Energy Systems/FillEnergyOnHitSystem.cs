using Damage;
using Health;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateBefore(typeof(HandleHitBufferSystem))]
[UpdateInGroup(typeof(CombatSystemGroup))]
public partial struct FillEnergyOnHitSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnergyBarComponent>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        ResetHasChangedEnergy(ref state);
        HandleEnergyReset(ref state);
        HandleEnergyFill(ref state);
    }
    
    [BurstCompile]
    private void ResetHasChangedEnergy(ref SystemState state)
    {
        foreach (var (_, entity) in SystemAPI.Query<HasChangedEnergy>().WithEntityAccess())
        {
            state.EntityManager.SetComponentEnabled<HasChangedEnergy>(entity, false);
        }
    }
    
    [BurstCompile]
    private void HandleEnergyReset(ref SystemState state)
    {
        foreach (var ( bar, entity) in SystemAPI
            .Query<RefRW<EnergyBarComponent>>()
            .WithAll<ResetEnergyTag>()
            .WithEntityAccess())
        {
            state.EntityManager.SetComponentEnabled<ResetEnergyTag>(entity, false);
            var energyBar = bar;
            float energyLoss = -energyBar.ValueRO.MaxEnergy; 
            FillEnergyBarWithRef(ref state, energyBar, ref energyLoss, entity);
        }
    }

    [BurstCompile]
    private void HandleEnergyFill(ref SystemState state)
    {
        // direct hits
        FillEnergyFromDirectActiveHits(ref state);
        FillEnergyFromDirectPassiveHits(ref state);
        
        // ability hits
        FillEnergyFromPassiveAbilities(ref state);
    }

    [BurstCompile]
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
                
                var  bar = barToFill;
                FillEnergyBarWithRef(ref state, bar, ref totalEnergyFill, passiveEntity);
            } 
        }
    }

    [BurstCompile]
    private void FillEnergyFromDirectPassiveHits(ref SystemState state)
    {
        // Fill passive energy bars - direct hit
        foreach (var (energyFill, energyBar, weapon, hitBuffer, entity) in
            SystemAPI.Query<EnergyFillComponent, RefRW<EnergyBarComponent>, WeaponComponent, DynamicBuffer<HitBufferElement>>()
                .WithNone<ActiveWeapon>()
                .WithEntityAccess())
        {
            // exit out if energy bar is already fill
            var refRw = energyBar;
            if (refRw.ValueRO.IsFull) continue;

            // exit if hit buffer has not hit
            if (!HasHit(hitBuffer, out int hitCount)) continue;
            
            float totalEnergyFill = hitCount * energyFill.ActiveFillPerHit;
            FillEnergyBarWithRef(ref state, refRw, ref totalEnergyFill, entity);
        }
    }
    
    [BurstCompile]
    private void FillEnergyFromPassiveAbilities(ref SystemState state)
    {
        foreach (var (ownerWeapon, abilityHitBuffer, fillMod) in
            SystemAPI.Query<HasOwnerWeapon, DynamicBuffer<HitBufferElement>, EnergyFillComponent>())//.WithAll<HasChangedHP>())
        {
            Entity ownerEntity = ownerWeapon.OwnerEntity;
            
            // exit if owner is not assigned
            if (ownerEntity == Entity.Null) continue;
            
            bool ownerIsPassive = !ownerWeapon.OwnerWasActive;
            
            // exit if owner was not passive. 
            if (!ownerIsPassive) continue;

            EnergyBarComponent energyBar = state.EntityManager.GetComponentData<EnergyBarComponent>(ownerEntity);

            // exit early if the bar already has reached its max energy
            if (energyBar.IsFull) continue;

            if (!HasHit(abilityHitBuffer, out int hitCount)) continue;

            // calculate energy fill amount based on owners fill per hit and ability's fill per hit
            EnergyFillComponent ownerEnergyFill = state.EntityManager.GetComponentData<EnergyFillComponent>(ownerEntity);
            float abilityFillModifier = fillMod.PassiveFillPerHit;
            float ownerFillPerHit = ownerEnergyFill.PassiveFillPerHit;
            
            // fill bar
            float totalEnergyChange = hitCount * ownerFillPerHit * abilityFillModifier;
            FillEnergyBarWithEM(ref state, ref energyBar, ref totalEnergyChange, ref ownerEntity);
        }
    }

    [BurstCompile]
    private bool HasHit(DynamicBuffer<HitBufferElement> hitBuffer, out int hitCount)
    {
        hitCount = GetHitCount(hitBuffer);
        return hitCount > 0;
    }

    [BurstCompile]
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

    private static void FillEnergyBarWithRef(ref SystemState state, RefRW<EnergyBarComponent> energyBar, ref float energyFill, Entity entity)
    {
        var oldEnergy = energyBar.ValueRO.CurrentEnergy;
        var newEnergy = GetNewEnergy(ref energyFill, ref oldEnergy, ref energyBar.ValueRW.MaxEnergy);
        float deltaEnergy = newEnergy - oldEnergy;

        energyBar.ValueRW.CurrentEnergy = newEnergy;
        
        UpdateHasChangedEnergy(ref state, ref entity, ref deltaEnergy);
    }
    
    [BurstCompile]
    private static void FillEnergyBarWithEM(ref SystemState state, ref EnergyBarComponent energyBar, ref float energyFill, ref Entity entity)
    {
        float oldEnergy = energyBar.CurrentEnergy;
        float newEnergy = GetNewEnergy(ref energyFill, ref  oldEnergy, ref energyBar.MaxEnergy);
        float deltaEnergy = newEnergy - oldEnergy;

        EnergyBarComponent newEnergyBar = new EnergyBarComponent 
        {
            CurrentEnergy = newEnergy,
            MaxEnergy = energyBar.MaxEnergy
        };
        state.EntityManager.SetComponentData(entity, newEnergyBar);
        
        UpdateHasChangedEnergy(ref state, ref entity, ref deltaEnergy);
    }

    [BurstCompile]
    private static float GetNewEnergy(ref float energyFill, ref float currentEnergy, ref float maxEnergy)
    {
        float newEnergy = currentEnergy + energyFill;
        if (newEnergy >= maxEnergy)
            newEnergy = maxEnergy;
        return newEnergy;
    }

    [BurstCompile]
    private static void UpdateHasChangedEnergy(ref SystemState state, ref Entity entity, ref float changeValue)
    {
        state.EntityManager.SetComponentEnabled<HasChangedEnergy>(entity, true);
        state.EntityManager.SetComponentData(entity, new HasChangedEnergy {Value = changeValue});
    }
}
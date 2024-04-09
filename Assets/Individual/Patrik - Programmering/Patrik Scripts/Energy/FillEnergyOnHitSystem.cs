using Damage;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateBefore(typeof(HandleHitBufferSystem))]
public partial struct FillEnergyOnHitSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnergyBarComponent>();
        state.RequireForUpdate<EnergyFillComponent>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        // Fill active energy bars


        // Fill passive energy bars
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

                float newEnergy = energyBar.ValueRO.CurrentEnergy + energyFill.FillPerHit;
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
}

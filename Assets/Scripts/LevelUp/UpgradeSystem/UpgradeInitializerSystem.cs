using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.Content;
using UnityEngine;

[UpdateBefore(typeof(UpgradeUISystem))]
public partial struct UpgradeInitializerSystem : ISystem
{
    
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UpgradeChoice>();
        EventManager.OnSceneChange += OnSceneChange;
    }
    
    public void OnDestroy(ref SystemState state)
    {
        EventManager.OnSceneChange -= OnSceneChange;
    }
    
    private void OnSceneChange(MenuButtonSelection arg0)
    {
        // Reset Upgrade choice
        if (SystemAPI.TryGetSingletonRW<UpgradeChoice>(out RefRW<UpgradeChoice> choice))
        {
            choice.ValueRW.IsHandled = true;
        }
    }
    
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
    
        var choice = SystemAPI.GetSingletonRW<UpgradeChoice>();
        choice.ValueRW.IsHandled = true;
    }
}

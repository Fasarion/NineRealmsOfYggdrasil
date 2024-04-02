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
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var choice = SystemAPI.GetSingletonRW<UpgradeChoice>();
        choice.ValueRW.IsHandled = true;
    }
}

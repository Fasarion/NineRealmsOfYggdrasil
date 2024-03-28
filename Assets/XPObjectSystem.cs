using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

//TODO: set update order
[BurstCompile]
public partial struct XPObjectSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerXP>();
        state.RequireForUpdate<PlayerLevel>();
        state.RequireForUpdate<PlayerLevelingConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<PlayerLevelingConfig>();
        var level = SystemAPI.GetSingletonRW<PlayerLevel>();
        var xp = SystemAPI.GetSingletonRW<PlayerXP>();
        
    }
}

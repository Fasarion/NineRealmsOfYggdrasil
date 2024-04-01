using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial struct LevelUpSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerLevel>();
        state.RequireForUpdate<PlayerXP>();
        state.RequireForUpdate<PlayerLevelingConfig>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var level = SystemAPI.GetSingletonRW<PlayerLevel>();
        var xp = SystemAPI.GetSingletonRW<PlayerXP>();
        var config = SystemAPI.GetSingleton<PlayerLevelingConfig>();
    }
}

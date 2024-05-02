using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(XPObjectSystem))]
[BurstCompile]
public partial struct LevelUpSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerLevel>();
        state.RequireForUpdate<PlayerXP>();
        state.RequireForUpdate<PlayerLevelingConfig>();
        state.RequireForUpdate<PlayerSkillpoints>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var level = SystemAPI.GetSingletonRW<PlayerLevel>();
        var xp = SystemAPI.GetSingletonRW<PlayerXP>();
        var config = SystemAPI.GetSingleton<PlayerLevelingConfig>();
        var skillpoints = SystemAPI.GetSingletonRW<PlayerSkillpoints>();

        int currentLevel = level.ValueRO.Value;
        int currentXP = xp.ValueRO.XPValue;
        int xpNeededToLevel = 0;
        int baseXPNeeded = config.BaseXPNeeded;
        int addedXPNeeded = config.AddedXPNeededPerLevel;

        if (currentLevel < 1)
        {
            xpNeededToLevel = baseXPNeeded;
        }
        else
        {
            int cumulativeValue = 0;
            xpNeededToLevel = baseXPNeeded;
            
            for (int i = 0; i < currentLevel; i++)
            {
                cumulativeValue += addedXPNeeded;
                xpNeededToLevel += cumulativeValue;
            }
        }

        xp.ValueRW.XPNeededToLevelUp = xpNeededToLevel;

        if (currentXP >= xpNeededToLevel)
        {
            level.ValueRW.Value = currentLevel + 1;
            xp.ValueRW.XPValue = currentXP - xpNeededToLevel;
            skillpoints.ValueRW.Value++;
        }
    }
}

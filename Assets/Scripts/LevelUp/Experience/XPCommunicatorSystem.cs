using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial class XPCommunicatorSystem : SystemBase
{
    private int _cachedXP = int.MaxValue;
    protected override void OnUpdate()
    {
        var level = SystemAPI.GetSingleton<PlayerLevel>();
        var xp = SystemAPI.GetSingleton<PlayerXP>();

        if (_cachedXP == xp.XPValue) return;

        _cachedXP = xp.XPValue;

        var xpInfo = new ExperienceInfo
        {
            currentXP = _cachedXP,
            experienceNeededToLevelUp = xp.XPNeededToLevelUp,
            currentLevel = level.Value,
        };
        
        EventManager.OnPlayerExperienceChange?.Invoke(xpInfo);
    }
}

public struct ExperienceInfo
{
    public int currentXP;
    public int experienceNeededToLevelUp;
    public int currentLevel;
}

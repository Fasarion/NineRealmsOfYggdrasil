using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial class XPCommunicatorSystem : SystemBase
{
    private int _cachedXP;
    private bool isInitialized;
    protected override void OnUpdate()
    {
        var level = SystemAPI.GetSingleton<PlayerLevel>();
        var xp = SystemAPI.GetSingleton<PlayerXP>();

        if (!isInitialized) _cachedXP = int.MaxValue;

        if (_cachedXP == xp.XPValue && _cachedXP != 0) return;

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

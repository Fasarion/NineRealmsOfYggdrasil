using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(LevelUpSystem))]
public partial class XPCommunicatorSystem : SystemBase
{
    private int _cachedXP;
    private bool isInitialized;
    private float startUpTimer;
    protected override void OnUpdate()
    {
        if (!isInitialized)
        {
            if (startUpTimer < 1)
            {
                startUpTimer += SystemAPI.Time.DeltaTime;
                return;
            }
            
            _cachedXP = int.MaxValue;
            isInitialized = true;
            return;
        }
        
        
        var level = SystemAPI.GetSingleton<PlayerLevel>();
        var xp = SystemAPI.GetSingleton<PlayerXP>();
        

        
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

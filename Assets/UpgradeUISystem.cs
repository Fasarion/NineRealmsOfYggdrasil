using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial class UpgradeUISystem : SystemBase
{
    public Action<UpgradeObject[]> OnUpgradeUIDisplayCall;
    public Action<UpgradeObject> OnUpgradeCardButtonPressed;
    private int _cachedLevel;
    private bool _isUpgradeUIActive;
    private bool _isUpgradeUIButtonActive;
    private bool _isUpgradeUIGenerated;

    private UpgradePoolManager _pool = null;
    private UpgradeObject[] _upgradeObjects;
    private bool _shouldGenerateNewSkillChoices;
    
    protected override void OnUpdate()
    {
        var level = SystemAPI.GetSingleton<PlayerLevel>();
        int currentLevel = level.Value;
        
        CheckForLevelUp(currentLevel);
        
    }

    private void CheckForLevelUp(int currentLevel)
    {
        if (_cachedLevel == currentLevel) return;

        _cachedLevel = currentLevel;
        
        GenerateUpgradeUIChoices();
        
        
        
        OnUpgradeUIDisplayCall?.Invoke(_upgradeObjects);
        
    }


    private void GenerateUpgradeUIChoices()
    {
        if (_pool == null)
        {
            _pool = UpgradePoolManager.Instance;
        }

        _upgradeObjects = _pool.GetRandomUpgrades();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial class UpgradeUISystem : SystemBase
{
    public Action<UpgradeObject[]> OnUpgradeUIDisplayCall;
    private int _cachedLevel = 0;
    private bool _isUpgradeUIActive;
    private bool _isUpgradeUIButtonActive;
    private bool _isUpgradeUIGenerated;

    private UpgradePoolManager _pool = null;
    private UpgradeObject[] _upgradeObjects;
    private bool _shouldGenerateNewSkillChoices;

    private UpgradeCardUIManager _uiManager;
    
    protected override void OnUpdate()
    {
        var level = SystemAPI.GetSingleton<PlayerLevel>();
        
        int currentLevel = level.Value;
        
        SubscribeToManager();
        
        CheckForLevelUp(currentLevel);
        
    }

    private void CheckForLevelUp(int currentLevel)
    {
        if (_cachedLevel == currentLevel) return;

        _cachedLevel = currentLevel;
        
        GenerateUpgradeUIChoices();
        
        OnUpgradeUIDisplayCall?.Invoke(_upgradeObjects);
        
    }

    private void SubscribeToManager()
    {
        if (_uiManager == null)
        {
            _uiManager = UpgradeCardUIManager.Instance;
            _uiManager.OnUpgradeChosen += RecieveUpgradeChoice;
        }

        if (_uiManager == null)
        {
            Debug.LogError("Missing UpgradeCardUIManager. Ask programmers for help!");
            return;
        }
    }

    public void RecieveUpgradeChoice(int index)
    {
        var choice = SystemAPI.GetSingletonRW<UpgradeChoice>();
        choice.ValueRW.ChoiceIndex = index;
        choice.ValueRW.IsHandled = false;
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

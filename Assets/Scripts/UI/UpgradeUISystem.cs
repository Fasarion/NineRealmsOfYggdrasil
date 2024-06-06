using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public enum UpgradePoolType
{
    Commerce,
    BirdUnlock,
    HammerUnlock,
}

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

    protected override void OnStartRunning()
    {
        EventManager.OnSceneChange += OnSceneChange;
    }

    protected override void OnStopRunning()
    {
        EventManager.OnSceneChange -= OnSceneChange;
    }

    private void OnSceneChange(MenuButtonSelection selection)
    {
        ResetSystem(selection);
    }

    private void ResetSystem(MenuButtonSelection menuButtonSelection)
    {
        _cachedLevel = 0;
        
        bool playerLevelExists = SystemAPI.TryGetSingletonRW<PlayerLevel>(out RefRW<PlayerLevel> level);
        if (!playerLevelExists)
        {
            // No player level found";
            return;
        }

        level.ValueRW.Value = 0;

        switch (menuButtonSelection)
        {
            case MenuButtonSelection.ExitGame:
                OnUpgradeUIDisplayCall = null;
                _isUpgradeUIActive = false;
                _isUpgradeUIButtonActive = false;
                _isUpgradeUIGenerated = false;

                _pool = null;
                _upgradeObjects = null;
                _shouldGenerateNewSkillChoices = false;

                _uiManager = null;
                break;
        }
        
        
    }

    protected override void OnUpdate()
    {
        bool playerLevelExists = SystemAPI.TryGetSingleton<PlayerLevel>(out PlayerLevel level);
        if (!playerLevelExists)
        {
            // No player level found";
            return;
        }

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

            if (_uiManager) _uiManager.OnUpgradeChosen += RecieveUpgradeChoice;
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

    private UpgradePoolType GetUpgradePoolType()
    {
        UpgradePoolType poolType;

        if (_cachedLevel == 2) poolType = UpgradePoolType.HammerUnlock;
        else if (_cachedLevel == 5) poolType = UpgradePoolType.BirdUnlock;
        else poolType = UpgradePoolType.Commerce;

        return poolType;
    }

    private void GenerateUpgradeUIChoices()
    {
        if (_pool == null)
        {
            _pool = UpgradePoolManager.Instance;
        }

        UpgradePoolType poolType = GetUpgradePoolType();

        _upgradeObjects = _pool.GetRandomUpgrades(poolType);
    }
}
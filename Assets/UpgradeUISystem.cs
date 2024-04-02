using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial class UpgradeUISystem : SystemBase
{
    public Action OnUpgradeUIButtonDisplay;
    public Action OnUpgradeUIButtonPressed;
    public Action OnUpgradeCardButtonPressed;
    private int _cachedLevel;
    private bool _isUpgradeUIActive;
    private bool _isUpgradeUIButtonActive;
    private bool _isUpgradeUIGenerated;
    private int _currentSkillpoints;
    
    
    protected override void OnUpdate()
    {
        var level = SystemAPI.GetSingleton<PlayerLevel>();
        int currentLevel = level.Value;
        var skillpoints = SystemAPI.GetSingleton<PlayerSkillpoints>();
        _currentSkillpoints = skillpoints.Value;
        var upgradeButton = SystemAPI.GetSingleton<UpgradeUIButtonPress>();
        bool isUpgradeUIButtonPressed = upgradeButton.isPressed;
        
        CheckForLevelUp(currentLevel);
        
        CheckForSkillpoints(_currentSkillpoints);

        CheckForOpenCloseUIPress(isUpgradeUIButtonPressed);
    }

    private void CheckForLevelUp(int currentLevel)
    {
        if (_cachedLevel == currentLevel) return;

        _cachedLevel = currentLevel;
        
    }

    private void CheckForSkillpoints(int currentSkillpoints)
    {
        if ((currentSkillpoints <= 0 && _isUpgradeUIButtonActive) ||
            (currentSkillpoints > 0 && !_isUpgradeUIButtonActive))
        {
            OnUpgradeUIButtonDisplay?.Invoke();
            if (_isUpgradeUIButtonActive) _isUpgradeUIButtonActive = false;
            else _isUpgradeUIButtonActive = true;
        }
    }

    private void CheckForOpenCloseUIPress(bool isPressed)
    {
        if (!isPressed) return;
        
        OnUpgradeUIButtonPressed?.Invoke();
    }

    private void CheckForUpgradeUICardButtonPress(bool isPressed)
    {
        if (!isPressed) return;
        
        
    }

    private void GenerateUpgradeUIChoices()
    {
        
    }
}

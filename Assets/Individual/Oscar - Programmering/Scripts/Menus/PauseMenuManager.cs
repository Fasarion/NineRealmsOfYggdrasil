using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject audioSettingsMenu;

    public void Awake()
    {
        DeactivatePauseMenu();
        AudioSettingsDeactivated();
    }

    public void PauseGame()
    {
        EventManager.OnPause?.Invoke(PauseType.PauseScreen);
    }

    public void OnEnable()
    {
        EventManager.OnPause += OnPause;
        EventManager.OnUnpause += OnUnpause;
    }
    
    public void OnDisable()
    {
        EventManager.OnPause -= OnPause;
        EventManager.OnUnpause -= OnUnpause;
    }

    public void AudioSettingsActivated()
    {
        audioSettingsMenu.SetActive(true);
    }
    
    public void AudioSettingsDeactivated()
    {
        audioSettingsMenu.SetActive(false);
    }
    
    public void OnPause(PauseType pauseType)
    {
        if (pauseType == PauseType.PauseScreen)
        {
            ActivatePauseMenu();
        }
    } 
    
    private void OnUnpause(PauseType pauseType)
    {
        if (pauseType == PauseType.PauseScreen)
        {
            DeactivatePauseMenu();
        }
    }

    public void UnpauseGame()
    {
        EventManager.OnUnpause?.Invoke(PauseType.PauseScreen);
        DeactivatePauseMenu();
    }

    private void ActivatePauseMenu()
    {
        pauseMenuPanel.SetActive(true);
    }
    
    private void DeactivatePauseMenu()
    {
        pauseMenuPanel.SetActive(false);
    }
}
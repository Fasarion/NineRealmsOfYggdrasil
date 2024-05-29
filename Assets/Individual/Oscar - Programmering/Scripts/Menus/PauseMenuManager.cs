using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;

    public void Awake()
    {
        DeactivatePauseMenu();
    }

    public void PauseGame()
    {
        EventManager.OnPause?.Invoke();
       
    }

    public void OnEnable()
    {
        EventManager.OnPause += OnPause;
    }
    
    public void OnDisable()
    {
        EventManager.OnPause -= OnPause;
    }

    public void OnPause()
    {
        ActivatePauseMenu();
    }

    public void UnpauseGame()
    {
        EventManager.OnUnpause?.Invoke();
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
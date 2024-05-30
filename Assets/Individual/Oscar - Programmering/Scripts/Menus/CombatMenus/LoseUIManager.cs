using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseUIManager : MonoBehaviour
{

    [SerializeField] private GameObject loseUIPanel;
    public void OnEnable()
    {
        EventManager.OnPlayerDeath += OnPlayerDeath;
    }
    public void OnDisable()
    {
        EventManager.OnPlayerDeath -= OnPlayerDeath;
    }

    public void OnPlayerDeath()
    {
        ActivateLoseUIMenu();
    }

    public void Awake()
    {
        DeactivateLoseUIMenu();
    }

    private void ActivateLoseUIMenu()
    {
        loseUIPanel.SetActive(true);
    }
    private void DeactivateLoseUIMenu()
    {
        loseUIPanel.SetActive(false);
    }
}

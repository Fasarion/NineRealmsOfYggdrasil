using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarModel : ElementMVC
{
    public int currentHealth;
    public int maxHealth;
    [NonSerialized]public List<float> levels;
    
    private void OnEnable()
    {
        EventManager.OnPlayerHealthSet += OnHealthSet;
    }
    
    private void OnDisable()
    {
        EventManager.OnPlayerHealthSet += OnHealthSet;
    }

    private void OnHealthSet(PlayerHealthData healthData)
    {
        currentHealth = (int)healthData.currentHealth;
        maxHealth = (int) healthData.maxHealth;
        app.Notify(NotificationMVC.SetHealthModel, this, currentHealth, maxHealth);
    }


    //PlayerHealthData healthData = (PlayerHealthData)p_data[0];
               
    //app.model.healthBarModel.currentHealth = (int)healthData.currentHealth;
    //app.model.healthBarModel.maxHealth = (int) healthData.maxHealth;
}

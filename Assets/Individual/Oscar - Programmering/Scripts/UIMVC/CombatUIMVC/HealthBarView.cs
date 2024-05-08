using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarView : ElementMVC
{
    public void OnEnable()
    {
        EventManager.OnPlayerHealthSet += OnHealthSet;
    }
    public void OnDisable()
    {
        EventManager.OnPlayerHealthSet += OnHealthSet;
    }

    public void OnHealthSet(PlayerHealthData healthData)
    {
        app.Notify(NotificationMVC.SetHealth,this, healthData);
    }
}

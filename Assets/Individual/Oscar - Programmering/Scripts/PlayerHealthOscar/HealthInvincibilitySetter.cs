using System;
using System.Collections;
using System.Collections.Generic;
using Health;
using UnityEngine;

public class HealthInvincibilitySetter : MonoBehaviour
{
    public HealthInvincibilitySO invincibilitySo;
    public void OnEnable()
    {
        EventManager.OnInitialPlayerHealthSet += OnPlayerHealthSet;
    }

    private void OnPlayerHealthSet(PlayerHealthData arg0)
    {
        EventManager.OnPlayerDamageReductionSet?.Invoke(invincibilitySo.damageReductionValue);  
    }

    public void OnDisable()
    {
        EventManager.OnInitialPlayerHealthSet -= OnPlayerHealthSet;
    }
}

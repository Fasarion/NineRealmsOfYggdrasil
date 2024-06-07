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
        EventManager.OnPlayerHealthSet += OnPlayerHealthSet;
    }

    private void OnPlayerHealthSet(PlayerHealthData arg0)
    {
        Debug.Log($"Invoke damage reduction event with value: {invincibilitySo.damageReductionValue}");
        EventManager.OnPlayerDamageReductionSet?.Invoke(invincibilitySo.damageReductionValue);  
    }

    public void OnDisable()
    {
        EventManager.OnPlayerHealthSet -= OnPlayerHealthSet;
    }
}

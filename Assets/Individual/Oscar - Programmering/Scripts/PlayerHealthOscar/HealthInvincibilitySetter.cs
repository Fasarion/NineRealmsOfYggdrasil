using System;
using System.Collections;
using System.Collections.Generic;
using Health;
using UnityEngine;

public class HealthInvincibilitySetter : MonoBehaviour
{
    public HealthInvincibilitySO invincibilitySo;
    public void Awake()
    {
        //EventManager.OnPlayerDamageReductionSet?.Invoke(invincibilitySo.damageReductionValue);    
    }
}

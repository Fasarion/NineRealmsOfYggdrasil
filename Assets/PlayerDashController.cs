using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashController : MonoBehaviour
{
    [SerializeField] private GameObject objectToEnable;

    private void OnEnable()
    {
        EventManager.OnDashBegin += OnDashBegin;
        EventManager.OnDashEnd += OnDashEnd;
    }
    
    private void OnDisable()
    {
        EventManager.OnDashBegin -= OnDashBegin;
        EventManager.OnDashEnd -= OnDashEnd;
    }

    private void OnDashBegin()
    {
        SetDashEffectEnabled(true);
    }

    private void OnDashEnd()
    {
        SetDashEffectEnabled(false);
    }

    private void SetDashEffectEnabled(bool enable)
    {
        objectToEnable.SetActive(enable);
    }
}

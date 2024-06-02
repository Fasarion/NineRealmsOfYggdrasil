using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDisabler : MonoBehaviour
{

    public List<GameObject> UserInterfacesToDisable;

    public void OnEnable()
    {
        EventManager.OnEnableUI += OnEnableUI;
        EventManager.OnDisableUI += OnDisableUI;
    }
    
    public void OnDisable()
    {
        EventManager.OnEnableUI -= OnEnableUI;
        EventManager.OnDisableUI -= OnDisableUI;
    }

    public void OnEnableUI()
    {
        for (int i = 0; i < UserInterfacesToDisable.Count; i++)
        {
            UserInterfacesToDisable[i].SetActive(true);
        }
    }
    public void OnDisableUI()
    {
        for (int i = 0; i < UserInterfacesToDisable.Count; i++)
        {
            UserInterfacesToDisable[i].SetActive(false);
        }
    }
    
    
}

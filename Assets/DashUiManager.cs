using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DashUiManager : MonoBehaviour
{
    [SerializeField] private DashUIIconBehaviour dashIconPrefab;
    private List<DashUIIconBehaviour> dashIcons = new List<DashUIIconBehaviour>();

    private void OnEnable()
    {
        EventManager.OnDashInfoUpdate += OnDashInfoUpdate;
    }
    
    private void OnDisable()
    {
        EventManager.OnDashInfoUpdate -= OnDashInfoUpdate;
    }

    private void OnDashInfoUpdate(DynamicBuffer<DashInfoElement> dashBuffer)
    {
        while (dashIcons.Count < dashBuffer.Length)
        {
            Debug.Log("Add dash icon");
            
            var dashIcon = Instantiate(dashIconPrefab, transform);
            dashIcons.Add(dashIcon);
        }

        for (int i = 0; i < dashIcons.Count; i++)
        {
            dashIcons[i].UpdateInfo(dashBuffer[i]);
        }
    }
}
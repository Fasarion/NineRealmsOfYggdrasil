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
        while (dashIcons.Count != dashBuffer.Length)
        {
            // Add more dashes
            if (dashIcons.Count < dashBuffer.Length)
            {
                var newDashIcon = Instantiate(dashIconPrefab, transform);
                dashIcons.Add(newDashIcon);
            }
            // remove dashes
            else
            {
                int lastIndex = dashIcons.Count - 1;
                
                var lastDashIcon = dashIcons[lastIndex];
                Destroy(lastDashIcon.gameObject);
                dashIcons.RemoveAt(lastIndex);
            }
        }
        
        // update dashes
        for (int i = 0; i < dashIcons.Count; i++)
        {
            dashIcons[i].UpdateInfo(dashBuffer[i]);
        }
    }
}
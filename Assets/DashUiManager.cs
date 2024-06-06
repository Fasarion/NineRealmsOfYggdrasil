using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class DashUiManager : MonoBehaviour
{
    [SerializeField] private DashUIIconBehaviour dashIconPrefab;
    [SerializeField] private Transform dashIconsParent;
    [SerializeField] private Image inputIcon;
    
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
                var newDashIcon = Instantiate(dashIconPrefab, dashIconsParent);
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
        
        // update dash icons
        for (int i = 0; i < dashIcons.Count; i++)
        {
            int index = i;
            dashIcons[index].UpdateInfo(dashBuffer[index]);
        }
        
        // update input icon
        bool hasDash = false;
        foreach (var bufferElement in dashBuffer)
        {
            if (bufferElement.Value.Ready)
            {
                hasDash = true;
                break;
            }
        }
        
        inputIcon.gameObject.SetActive(hasDash);
    }
}
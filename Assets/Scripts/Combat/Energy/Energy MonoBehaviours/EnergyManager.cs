using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private WeaponEnergyUIBehaviour weaponIconPrefab;
    
    [SerializeField] private int distanceBetweenIcons = 100;
    private List<WeaponEnergyUIBehaviour> weaponIcons = new ();
    private Dictionary<WeaponType, WeaponEnergyUIBehaviour> weaponIconMap = new ();
    private int iconCount => weaponIcons.Count;

    private void Awake()
    {
        EventManager.OnSetupWeapon += SetupWeapon;
        EventManager.OnWeaponSwitch += OnWeaponSwitch;
        EventManager.OnEnergyChange += OnEnergyChange;
    }
    
    private void OnDisable()
    {
        EventManager.OnSetupWeapon -= SetupWeapon;
        EventManager.OnWeaponSwitch -= OnWeaponSwitch;
        EventManager.OnEnergyChange -= OnEnergyChange;
    }

    private void OnEnergyChange(WeaponType weaponType, float currentEnergy, float maxEnergy)
    {
        var icon = weaponIconMap[weaponType];
        icon.UpdateBar(currentEnergy, maxEnergy);
    }

    private void SetupWeapon(WeaponSetupData data)
    {
        Vector3 iconPosition = transform.position + Vector3.right * distanceBetweenIcons * iconCount;
        var icon = Instantiate(weaponIconPrefab, transform);
        icon.transform.position = iconPosition;
        
        icon.Setup(data.WeaponBehaviour);
        icon.SetActiveWeapon(data.Active);
        
        weaponIcons.Add(icon);
        weaponIconMap.Add(data.WeaponType, icon);
    }

    private void OnWeaponSwitch(WeaponBehaviour activeWeapon)
    {
        foreach (var weaponIcon in weaponIcons)
        {
            bool isActive = weaponIcon.Weapon == activeWeapon;
            weaponIcon.SetActiveWeapon(isActive);
        }
    }
}
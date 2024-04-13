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
    private int iconCount => weaponIcons.Count;

    private void Awake()
    {
        EventManager.OnSetupWeapon += SetupWeapon;
        EventManager.OnWeaponSwitch += OnWeaponSwitch;
    }
    
    private void OnDisable()
    {
        EventManager.OnSetupWeapon -= SetupWeapon;
        EventManager.OnWeaponSwitch -= OnWeaponSwitch;
    }
    private void SetupWeapon(WeaponBehaviour weapon, bool active)
    {
        Vector3 iconPosition = transform.position + Vector3.right * distanceBetweenIcons * iconCount;
        var icon = Instantiate(weaponIconPrefab, transform);
        icon.transform.position = iconPosition;
        
        icon.Setup(weapon);
        icon.SetActiveWeapon(active);
        
        weaponIcons.Add(icon);
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
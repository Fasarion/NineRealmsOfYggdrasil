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
        EventManager.OnAllWeaponsSetup += SetupWeapon;
        EventManager.OnWeaponSwitch += OnWeaponSwitch;
        EventManager.OnEnergyChange += OnEnergyChange;
    }
    
    private void OnDisable()
    {
        EventManager.OnAllWeaponsSetup -= SetupWeapon;
        EventManager.OnWeaponSwitch -= OnWeaponSwitch;
        EventManager.OnEnergyChange -= OnEnergyChange;
    }

    private void OnEnergyChange(WeaponType weaponType, float currentEnergy, float maxEnergy)
    {
        var icon = weaponIconMap[weaponType];
        icon.UpdateBar(currentEnergy, maxEnergy);
    }

    private void SetupWeapon(List<WeaponSetupData> allWeapons)
    {
        weaponIcons = new List<WeaponEnergyUIBehaviour>();
        weaponIconMap = new Dictionary<WeaponType, WeaponEnergyUIBehaviour>();
        for (int i = 0; i < allWeapons.Count; i++)
        {
            Vector3 iconPosition = transform.position + Vector3.right * distanceBetweenIcons * iconCount;
            var icon = Instantiate(weaponIconPrefab, transform);
            icon.transform.position = iconPosition;
        
            icon.Setup(allWeapons[i].WeaponBehaviour);
            icon.SetActiveWeapon(allWeapons[i].Active);
        
            weaponIcons.Add(icon);
            weaponIconMap.Add(allWeapons[i].WeaponType, icon);
        }
       
    }

    private void OnWeaponSwitch(WeaponSetupData activeWeapon, List<WeaponSetupData> allWeapons)
    {
        foreach (var weaponIcon in weaponIcons)
        {
            bool isActive = weaponIcon.Weapon == activeWeapon.WeaponBehaviour;
            weaponIcon.SetActiveWeapon(isActive);
        }
    }
}
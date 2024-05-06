using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIUltimateProgressBar : MonoBehaviour
{
    private enum SymbolType
    {
        Main,
        LeftInactive,
        RightInactive
    }
    
    public Slider slider;

    [SerializeField]private SymbolType symbolType;
    private WeaponType currentWeaponType;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void OnEnable()
    {
        EventManager.OnEnergyChange += OnEnergyChange;
        CombatUIWeaponHandler.onCurrentWeaponUpdated += OnCurrentWeaponUpdated;
    }

   

    public void OnDisable()
    {
        EventManager.OnEnergyChange -= OnEnergyChange;
        CombatUIWeaponHandler.onCurrentWeaponUpdated -= OnCurrentWeaponUpdated;
    }
    
    private void OnCurrentWeaponUpdated(WeaponType mainWeapon, WeaponType leftWeapon, WeaponType rightWeapon)
    {
        switch (symbolType)
        {
            case SymbolType.Main:
            {
                currentWeaponType = mainWeapon;
                break;
            }    
            case SymbolType.LeftInactive:
            {
                currentWeaponType = leftWeapon;
                break;
            } 
            case SymbolType.RightInactive:
            {
                currentWeaponType = rightWeapon;
                break;
            } 
        }
    }

    private void OnEnergyChange(WeaponType weaponTypeEnergyChanged, float currentEnergy, float maxEnergy)
    {
        if (currentWeaponType == weaponTypeEnergyChanged)
        {
            slider.value =  currentEnergy;
            slider.maxValue = maxEnergy;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

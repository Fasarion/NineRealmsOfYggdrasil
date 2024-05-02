using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIUltIcons : MonoBehaviour
{
    
    public List<CombatUIUltWeaponReadyHolder> ultWeaponReadyHolders;
    private List<WeaponType> weaponTypes;
    public CombatUISymbolHolder SymbolHolder;
    private void Awake()
    {
        weaponTypes = new List<WeaponType>();
    }

    // Start is called before the first frame update
    void Start()
    {
        weaponTypes.Clear();
    }

    public void OnEnable()
    {
        EventManager.OnEnergyChange += OnUltimate;
        EventManager.OnSetupWeapon += OnWeaponSetup;
    }
    
    public void OnDisable()
    {
        EventManager.OnEnergyChange -= OnUltimate;
        EventManager.OnSetupWeapon -= OnWeaponSetup;
    }

    public void OnWeaponSetup(WeaponBehaviour weaponBehaviour, bool activeWeapon)
    {
        weaponTypes.Add(weaponBehaviour.WeaponType); 
        if (weaponTypes.Count == 2) 
        {
            for (int i = 0; i < weaponTypes.Count; i++)
            {
                switch (weaponBehaviour.WeaponType)
                {
                    case WeaponType.Hammer:
                    {
                        ultWeaponReadyHolders[i].imageTarget.sprite = SymbolHolder.hammerSymbols[1];
                        break;
                    }
                    case WeaponType.Sword:
                    {
                
                        ultWeaponReadyHolders[i].imageTarget.sprite= SymbolHolder.swordSymbols[1];
                        break;
                    }
                    case WeaponType.Birds:
                    {
                
                        ultWeaponReadyHolders[i].imageTarget.sprite = SymbolHolder.birdSymbols[1];
                        break;
                    }
                    case WeaponType.Mead:
                    {
                        ultWeaponReadyHolders[i].imageTarget.sprite = SymbolHolder.meadSymbols[1];
                       
                        break;
                    }
                    default:
                    {
                        Debug.LogError("Error, the UltWeaponHolder was not updated correctly.");
                        break;
                    }
                }
                ultWeaponReadyHolders[i].imageTarget.SetNativeSize();
            }
        }
        

    }

    public void OnUltimateActivated(WeaponType weaponType)
    {
        
        for (int i = 0; i < ultWeaponReadyHolders.Count; i++)
        {
            if (ultWeaponReadyHolders[i].currentWeaponType == weaponType)
            {
                ultWeaponReadyHolders[i].objectToSet.SetActive(false);
            }
            
        }
    }

    public void OnUltimate(WeaponType weaponType, float currentEnergy, float maxEnergy)
    {
        //UltimateActivated
        if (currentEnergy == 0)
        {
            for (int i = 0; i < ultWeaponReadyHolders.Count; i++)
            {
                if (ultWeaponReadyHolders[i].currentWeaponType == weaponType)
                {
                    ultWeaponReadyHolders[i].objectToSet.SetActive(false);
                }
            
            }
        }
        
        //UltimateReady
        if (currentEnergy >= maxEnergy)
        {
            for (int i = 0; i < ultWeaponReadyHolders.Count; i++)
            {
                if (ultWeaponReadyHolders[i].currentWeaponType == weaponType)
                {
                    ultWeaponReadyHolders[i].objectToSet.SetActive(true);
                }
            }
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

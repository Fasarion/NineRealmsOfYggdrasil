using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIUltIcons : MonoBehaviour
{
    
    public List<CombatUIUltWeaponReadyHolder> ultWeaponReadyHolders;
    private List<WeaponType> weaponTypes;
    public CombatUISymbolHolder SymbolHolder;
    public int activeUltCounter;
    public TMP_Text ultimateActiveText;

    private int symbolCounter;
    private void Awake()
    {
        weaponTypes = new List<WeaponType>();
        symbolCounter = 0;
        activeUltCounter = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        weaponTypes.Clear();
        ultimateActiveText.gameObject.SetActive(false);
            
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

    public void OnWeaponSetup(WeaponSetupData weaponSetupData)
    {
        
        //if (weaponTypes.Count == 2) 
        //{
            //for (int i = 0; i < weaponTypes.Count; i++)
            //{
                switch (weaponSetupData.WeaponType)
                {
                    case WeaponType.Hammer:
                    {
                        ultWeaponReadyHolders[symbolCounter].imageTarget.sprite = SymbolHolder.hammerSymbols[1];
                        break;
                    }
                    case WeaponType.Sword:
                    {
                
                        ultWeaponReadyHolders[symbolCounter].imageTarget.sprite= SymbolHolder.swordSymbols[1];
                        break;
                    }
                    case WeaponType.Birds:
                    {
                        ultWeaponReadyHolders[symbolCounter].imageTarget.sprite = SymbolHolder.birdSymbols[1];
                        break;
                    }
                    case WeaponType.Mead:
                    {
                        ultWeaponReadyHolders[symbolCounter].imageTarget.sprite = SymbolHolder.meadSymbols[1];
                        
                        break;
                    }
                    default:
                    {
                        Debug.LogError("Error, the UltWeaponHolder was not updated correctly.");
                        break;
                    }
                }
                ultWeaponReadyHolders[symbolCounter].currentWeaponType = weaponSetupData.WeaponType;
                ultWeaponReadyHolders[symbolCounter].imageTarget.SetNativeSize();
                ultWeaponReadyHolders[symbolCounter].objectToSet.SetActive(false);
                symbolCounter++;
                
                //}
                //}


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
                    activeUltCounter--;
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
                    activeUltCounter++;
                }
            }
        }

        if (activeUltCounter > 0)
        {
            ultimateActiveText.gameObject.SetActive(true);
        }
        else
        {
            ultimateActiveText.gameObject.SetActive(false);
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

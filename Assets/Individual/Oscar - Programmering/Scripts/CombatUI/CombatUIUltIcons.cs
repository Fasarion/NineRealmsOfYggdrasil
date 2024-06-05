using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class WeaponEnergyData
{
    public WeaponType weaponType;
    public float currentEnergy;
    public float maxEnergy;
}
public class CombatUIUltIcons : MonoBehaviour
{
    
    public List<CombatUIUltWeaponReadyHolder> ultWeaponReadyHolders;
    private List<WeaponType> weaponTypes;
    public CombatUISymbolHolder SymbolHolder;
    public int activeUltCounter;
    public TMP_Text ultimateActiveText;

    private int symbolCounter;

    public List<WeaponEnergyData> weaponEnergyDataList;

    private WeaponEnergyData swordWeaponEnergyData;
    private WeaponEnergyData hammerWeaponEnergyData;
    private WeaponEnergyData birdWeaponEnergyData;

    private void Awake()
    {
        weaponEnergyDataList = new List<WeaponEnergyData>();
        
        swordWeaponEnergyData = new WeaponEnergyData{
            weaponType = WeaponType.Sword,
            currentEnergy = 0,
            maxEnergy = float.PositiveInfinity
        };
        
         hammerWeaponEnergyData = new WeaponEnergyData{
            weaponType = WeaponType.Hammer,
            currentEnergy = 0,
            maxEnergy = float.PositiveInfinity
        };
        
         birdWeaponEnergyData = new WeaponEnergyData{
            weaponType = WeaponType.Birds,
            currentEnergy = 0,
            maxEnergy = float.PositiveInfinity
        };
        weaponEnergyDataList.Add(swordWeaponEnergyData);
        weaponEnergyDataList.Add(hammerWeaponEnergyData);
        weaponEnergyDataList.Add(birdWeaponEnergyData);
        weaponTypes = new List<WeaponType>();
        symbolCounter = 0;
        activeUltCounter = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        weaponTypes.Clear();
       
        for (int i = 0; i < ultWeaponReadyHolders.Count; i++)
        {
            ultWeaponReadyHolders[i].StopSystem();
        }
        
       
        ultimateActiveText.gameObject.SetActive(false);

    }

    public void OnEnable()
    {
        EventManager.OnEnergyChange += OnUltimate;
        EventManager.OnSetupWeapon += OnWeaponSetup;
        EventManager.OnAllWeaponsSetup += OnAllWeaponsSetup;
        EventManager.OnWeaponSwitch += OnWeaponSwitch;
    }

    private void OnAllWeaponsSetup(List<WeaponSetupData> arg0)
    {
        
        symbolCounter = 0;
    }


    public void OnDisable()
    {
        EventManager.OnEnergyChange -= OnUltimate;
        EventManager.OnSetupWeapon -= OnWeaponSetup;
        EventManager.OnAllWeaponsSetup -= OnAllWeaponsSetup;
        EventManager.OnWeaponSwitch -= OnWeaponSwitch;
    }

    private void OnWeaponSwitch(WeaponSetupData arg0, List<WeaponSetupData> weaponSetupDataList)
    {
        for (int i = 0; i < weaponSetupDataList.Count; i++)
        {
            ultWeaponReadyHolders[i].currentWeaponType = weaponSetupDataList[i].WeaponType;
        }

       
        activeUltCounter = 0;
        for (int i = 0; i < ultWeaponReadyHolders.Count; i++)
        {
            for (int j = 0; j < weaponEnergyDataList.Count; j++)
            {
                if (ultWeaponReadyHolders[i].currentWeaponType == weaponEnergyDataList[j].weaponType)
                {
                    if (weaponEnergyDataList[j].currentEnergy >= weaponEnergyDataList[j].maxEnergy)
                    {
                        if (i == 0)
                        {
                            ultimateActiveText.gameObject.SetActive(true);
                        }
                        ultWeaponReadyHolders[i].PlaySystem();
                        
                        //ultWeaponReadyHolders[i].objectToSet.SetActive(true);
                        activeUltCounter++;
                    }
                    else
                    {
                        if (i == 0)
                        {
                            ultimateActiveText.gameObject.SetActive(false);
                        }
                        //if (weaponEnergyDataList[j].currentEnergy == 0)
                        //{
                            ultWeaponReadyHolders[i].StopSystem();
                            //ultimateActiveText.gameObject.SetActive(false);
                            //ultWeaponReadyHolders[i].objectToSet.SetActive(false);
                        //}
                        //ultWeaponReadyHolders[i].StopSystem();
                        //ultWeaponReadyHolders[i].objectToSet.SetActive(false);
                        //activeUltCounter--;
                    }
                }
            }
            
        }
        
        
        
    }


    public void OnWeaponSetup(WeaponSetupData weaponSetupData)
    {
        
        //if (weaponTypes.Count == 2) 
        //{
            //for (int i = 0; i < weaponTypes.Count; i++)
            //{
                /*switch (weaponSetupData.WeaponType)
                {
                    case WeaponType.Hammer:
                    {
                        //ultWeaponReadyHolders[symbolCounter].imageTarget.sprite = SymbolHolder.hammerSymbols[1];
                        break;
                    }
                    case WeaponType.Sword:
                    {
                
                        //ultWeaponReadyHolders[symbolCounter].imageTarget.sprite= SymbolHolder.swordSymbols[1];
                        break;
                    }
                    case WeaponType.Birds:
                    {
                        //ultWeaponReadyHolders[symbolCounter].imageTarget.sprite = SymbolHolder.birdSymbols[1];
                        break;
                    }
                    case WeaponType.Mead:
                    {
                        //ultWeaponReadyHolders[symbolCounter].imageTarget.sprite = SymbolHolder.meadSymbols[1];
                        
                        break;
                    }
                    default:
                    {
                        Debug.LogError("Error, the UltWeaponHolder was not updated correctly.");
                        break;
                    }
                }*/

                //WeaponEnergyData weaponEnergyData = new WeaponEnergyData();
                
                ultWeaponReadyHolders[symbolCounter].currentWeaponType = weaponSetupData.WeaponType;
                //ultWeaponReadyHolders[symbolCounter].imageTarget.SetNativeSize();
                ultWeaponReadyHolders[symbolCounter].StopSystem();
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
                ultWeaponReadyHolders[symbolCounter].StopSystem();
                //ultWeaponReadyHolders[i].objectToSet.SetActive(false);
            }
            
        }
    }

    public void OnUltimate(WeaponType weaponType, float currentEnergy, float maxEnergy)
    {
        
        for (int i = 0; i < weaponEnergyDataList.Count; i++)
        {
            if (weaponEnergyDataList[i].weaponType == weaponType)
            {
                weaponEnergyDataList[i].currentEnergy = currentEnergy;
                weaponEnergyDataList[i].maxEnergy = maxEnergy;
            }
        }
        //UltimateActivated
        //if (currentEnergy == 0)
        //{
            
            for (int i = 0; i < ultWeaponReadyHolders.Count; i++)
            {
                for (int j = 0; j < weaponEnergyDataList.Count; j++)
                {
                    if (ultWeaponReadyHolders[i].currentWeaponType == weaponEnergyDataList[j].weaponType)
                    {
                        if (weaponEnergyDataList[j].currentEnergy == 0)
                        {
                            if (i == 0)
                            {
                                ultimateActiveText.gameObject.SetActive(false);
                            }
                            ultWeaponReadyHolders[i].StopSystem();
                            
                            //ultWeaponReadyHolders[i].objectToSet.SetActive(false);
                            activeUltCounter--;
                        }
                        
                        if (weaponEnergyDataList[j].currentEnergy >= weaponEnergyDataList[j].maxEnergy)
                        {
                            if (i == 0)
                            {
                                ultimateActiveText.gameObject.SetActive(true);
                            }
                            ultWeaponReadyHolders[i].PlaySystem();
                            
                            //ultWeaponReadyHolders[i].objectToSet.SetActive(true);
                            activeUltCounter++;
                        }

                    }
                }
              
            
            }
        //}
        
        //UltimateReady
        //if (currentEnergy >= maxEnergy)
        //{
           /* for (int i = 0; i < ultWeaponReadyHolders.Count; i++)
            {
                
                
                if (ultWeaponReadyHolders[i].currentWeaponType == weaponEnergyDataList[i].weaponType)
                {
                    if (weaponEnergyDataList[i].currentEnergy >= weaponEnergyDataList[i].maxEnergy)
                    {
                        ultWeaponReadyHolders[symbolCounter].PlaySystem();
                        //ultWeaponReadyHolders[i].objectToSet.SetActive(true);
                        activeUltCounter++;
                    }
                }
            }*/
        //}

        /*if (activeUltCounter > 0)
        {
            ultimateActiveText.gameObject.SetActive(true);
        }
        else
        {
            ultimateActiveText.gameObject.SetActive(false);
        }*/
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

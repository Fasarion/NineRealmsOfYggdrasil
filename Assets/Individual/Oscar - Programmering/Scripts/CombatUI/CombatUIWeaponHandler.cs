using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class CombatUIWeaponHandler : ElementMVC
{

    public WeaponType currentWeapon;
    public WeaponSetupData mainWeaponSetupData;
    public WeaponType debugWeaponToUpdateTo;
    //WeaponType currentWeaponType;
    public CombatUIMainWeaponSymbol combatUIMainWeaponSymbol;

    public WeaponType currentLeftInactiveWeapon;
    public WeaponSetupData leftInactiveWeaponSetupData;
    public WeaponType currentRightInactiveWeapon;
    public WeaponSetupData rightInactiveWeaponSetupData;

    public static Action<WeaponSetupData, WeaponSetupData, WeaponSetupData> onCurrentWeaponUpdated;
    public static Action<WeaponSetupData, WeaponSetupData, WeaponSetupData> onStartingWeaponSet;
    //Placeholder for debugging purposes;
    //private WeaponType hammerWeapon = WeaponType.Hammer;
    //private WeaponType swordWeapon = WeaponType.Sword;
    //private WeaponType birdWeapon = WeaponType.Birds;
    
    //private WeaponType meadWeapon = WeaponType.Mead;
    //I am missing a class to reference to get the current types of weapons from the player
    public List<WeaponType> currentPlayerWeapons;

    private bool currentWeaponSet;
    private bool currentLeftWeaponSet;
    private bool currentRightWeaponSet;

    public void Awake()
    {
        currentWeaponSet = false;
        currentLeftWeaponSet = false;
        currentRightWeaponSet = false;
    }

    // Start is called before the first frame update
    void Start()
    {
 
        //currentPlayerWeapons.Add(hammerWeapon);
        //currentPlayerWeapons.Add(swordWeapon);
        //currentPlayerWeapons.Add(birdWeapon);
    }

    public void OnEnable()
    {
        EventManager.OnWeaponSwitch += OnWeaponSwitched;
        EventManager.OnSetupWeapon += OnSetupWeapon;
    }

   

    public void OnDisable()
    {
        EventManager.OnWeaponSwitch -= OnWeaponSwitched;
        EventManager.OnSetupWeapon -= OnSetupWeapon;
    }

    private void OnSetupWeapon(WeaponSetupData data)
    {
       
        //This does not account for the right inactive weapon
        if (data.Active)
        {
            currentWeapon = data.WeaponType;
            mainWeaponSetupData = data;
            //data.WeaponButtonIndex;
            currentWeaponSet = true;
        }
        else
        {
            if (currentLeftWeaponSet != true)
            {
                currentLeftInactiveWeapon = data.WeaponType;
                leftInactiveWeaponSetupData = data;
                currentLeftWeaponSet = true;
            }
            else
            {
                currentRightInactiveWeapon = data.WeaponType;
                rightInactiveWeaponSetupData = data;
                currentRightWeaponSet = true;
            }
        }

        if (currentWeaponSet && currentLeftWeaponSet && currentRightWeaponSet)
        {
            SetStartingWeapons();
        }
    }
    
    private void SetStartingWeapons()
    {
        
        onStartingWeaponSet?.Invoke(mainWeaponSetupData, leftInactiveWeaponSetupData, rightInactiveWeaponSetupData);
    }

    private void OnWeaponSwitched(WeaponBehaviour weaponBehaviour)
    {
        //Superhacky, but it should work
        if (weaponBehaviour.WeaponType == mainWeaponSetupData.WeaponType)
        {
            UpdateCurrentWeapon(mainWeaponSetupData);
        }
        else if (weaponBehaviour.WeaponType == leftInactiveWeaponSetupData.WeaponType)
        {
            UpdateCurrentWeapon(leftInactiveWeaponSetupData);
        }
        else if (weaponBehaviour.WeaponType == rightInactiveWeaponSetupData.WeaponType)
        {
            UpdateCurrentWeapon(rightInactiveWeaponSetupData);
        }
        else
        {
            Debug.LogError("You're trying to switch to a weapon that you do not have!");
        }
        //UpdateCurrentWeapon(weaponBehaviour.WeaponType);
    }

    public void DebugButtonPressed()
    {
        if (debugWeaponToUpdateTo == mainWeaponSetupData.WeaponType)
        {
            UpdateCurrentWeapon(mainWeaponSetupData);
        }
        else if (debugWeaponToUpdateTo == leftInactiveWeaponSetupData.WeaponType)
        {
            UpdateCurrentWeapon(leftInactiveWeaponSetupData);
        }
        else if (debugWeaponToUpdateTo == rightInactiveWeaponSetupData.WeaponType)
        {
            UpdateCurrentWeapon(rightInactiveWeaponSetupData);
        }
        else
        {
            Debug.LogError("You're trying to switch to a debug weapon that you do not have!");
        }
        //UpdateCurrentWeapon(debugWeaponToUpdateTo);
    }

  
    //We're going to need a list of the players current weapons here.
    public void UpdateCurrentWeapon(WeaponSetupData weaponSetupData)
    {
        //currentInactiveWeapons.Clear();
        //var selectableWeapons = new List<WeaponType>(currentPlayerWeapons);
        

        //Swap inactive and active weapons
        if(currentLeftInactiveWeapon == weaponSetupData.WeaponType)
        {
            currentLeftInactiveWeapon = currentWeapon;
            leftInactiveWeaponSetupData = mainWeaponSetupData;
            //Debug.Log("Swapped to left weapon");
        }
        else if (currentRightInactiveWeapon == weaponSetupData.WeaponType)
        {
            currentRightInactiveWeapon = currentWeapon;
            rightInactiveWeaponSetupData = mainWeaponSetupData;
            //Debug.Log("Swapped to right weapon");
        }
        else
        {
            Debug.Log("Something went wrong! You tried to set a weapon that was not one of your inactive weapons!");
        }
        
        currentWeapon = weaponSetupData.WeaponType;
        mainWeaponSetupData = weaponSetupData;
        
        
        //currentInactiveWeapons.AddRange(selectableWeapons);
                
        onCurrentWeaponUpdated?.Invoke(mainWeaponSetupData, leftInactiveWeaponSetupData, rightInactiveWeaponSetupData);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

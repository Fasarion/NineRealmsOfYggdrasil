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
    private List<WeaponSetupData> currentPlayerWeapons;

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
        EventManager.OnAllWeaponsSetup += OnSetupWeapon;
    }

   

    public void OnDisable()
    {
        EventManager.OnWeaponSwitch -= OnWeaponSwitched;
        EventManager.OnAllWeaponsSetup -= OnSetupWeapon;
    }

    private void OnSetupWeapon(List<WeaponSetupData> allWeapons)
    {
        //currentPlayerWeapons = new List<WeaponBehaviour>();
        currentPlayerWeapons = allWeapons;
      
        
        //This does not account for the right inactive weapon
        /*if (data.Active)
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
        }*/
        
        /*if (currentWeaponSet && currentLeftWeaponSet && currentRightWeaponSet)
        {
            SetStartingWeapons();
        }*/
        
            //mainWeaponSetupData = currentPlayerWeapons[0];
            //leftInactiveWeaponSetupData = currentPlayerWeapons[1];
            //rightInactiveWeaponSetupData = currentPlayerWeapons[2];
            //currentWeapon = currentPlayerWeapons[0].WeaponType; 
            //currentLeftInactiveWeapon = currentPlayerWeapons[1].WeaponType; 
            //currentRightInactiveWeapon = currentPlayerWeapons[2].WeaponType; 
            SetStartingWeapons();

    }
    
    private void SetStartingWeapons()
    {
        onStartingWeaponSet?.Invoke(currentPlayerWeapons[0], currentPlayerWeapons[1], currentPlayerWeapons[2]);
    }

    private void OnWeaponSwitched(WeaponSetupData currentWeaponData, List<WeaponSetupData> allWeapons)
    {
        currentPlayerWeapons = allWeapons;
        /*
        if (weaponBehaviour.WeaponType == currentLeftInactiveWeapon)
        {
            var last = currentPlayerWeapons[^1];
            for (int i = currentPlayerWeapons.Count-1; i > 0; i--)
            {
                 currentPlayerWeapons[i] = currentPlayerWeapons[i - 1 % currentPlayerWeapons.Count];
            }

            currentPlayerWeapons[0] = last;
        }
        else if (weaponBehaviour.WeaponType == currentRightInactiveWeapon)
        {
            var first = currentPlayerWeapons[0];
            for (int i = 0; i < currentPlayerWeapons.Count - 1; i++)
            {
                currentPlayerWeapons[i] =  currentPlayerWeapons[i + 1 % currentPlayerWeapons.Count] ;
            }
            currentPlayerWeapons[^1] = first;

            /*for (int i = currentPlayerWeapons.Count-1; i >= 1; i--)
            {
                nextValue = currentPlayerWeapons[i];
                //var nextValue = currentPlayerWeapons[i-1];
                
                currentPlayerWeapons[i - 1] =  currentPlayerWeapons[i];
                currentPlayerWeapons[i - 2] = nextValue;
            }

            //currentPlayerWeapons[currentPlayerWeapons.Count - 1] = first;
        }*/
        UpdateCurrentWeapon(currentPlayerWeapons);
        //UpdateCurrentWeapon();
        //Superhacky, but it should work
        /*if (weaponBehaviour.WeaponType == mainWeaponSetupData.WeaponType)
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
        }*/
        //UpdateCurrentWeapon(weaponBehaviour.WeaponType);
    }

    public void DebugButtonPressed()
    {
        
        /*if (debugWeaponToUpdateTo == mainWeaponSetupData.WeaponType)
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
        }*/
        //UpdateCurrentWeapon(debugWeaponToUpdateTo);
    }

  
    //We're going to need a list of the players current weapons here.
    public void UpdateCurrentWeapon(List<WeaponSetupData> weaponSetupData)
    {
        
        
        //currentInactiveWeapons.Clear();
        //var selectableWeapons = new List<WeaponType>(currentPlayerWeapons);
        

        
        //Swap inactive and active weapons
        /*if(currentLeftInactiveWeapon == weaponSetupData.WeaponType)
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
        }*/
        
        //currentWeapon = ;
        //currentLeftInactiveWeapon = weaponSetupData[1];
        //currentRightInactiveWeapon = weaponSetupData[2];
        //mainWeaponSetupData = weaponSetupData[0];
        //leftInactiveWeaponSetupData = weaponSetupData[1];
        //rightInactiveWeaponSetupData = weaponSetupData[2];
        
        
        //currentInactiveWeapons.AddRange(selectableWeapons);
                
        onCurrentWeaponUpdated?.Invoke(weaponSetupData[0], weaponSetupData[1], weaponSetupData[2]);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

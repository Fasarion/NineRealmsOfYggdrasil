using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum WeaponEnum
{
    Hammer,
    Bow,
    Spear,
        
}
public delegate void WeaponStateUpdate(string weaponToActivate, bool stateToSwitchTo); 
public class PassiveAttackBehaviour : MonoBehaviour
{

    public event WeaponStateUpdate WeaponStateUpdate;
    
    public List<GameObject> weaponList;
    
    
    //Use on pickups, level upgrade and item shops as needed
    public void PublishWeaponStateUpdate(string weaponToActivate, bool stateToSwitchTo)
    {
        WeaponStateUpdate?.Invoke(weaponToActivate, stateToSwitchTo);
    }

   

    
    private void Awake()
    {
        
        weaponList = new List<GameObject>();
        //var transforms = GetComponentsInChildren<Transform>().ToList();
        //for(int i = 1; i < transforms.Count; i++)
        //{
            //weaponList.Add(transforms[i].gameObject);
        //}
    }

    private void OnEnable()
    {
        WeaponStateUpdate += OnUpdateWeaponState;
    }

    private void OnDisable()
    {
        WeaponStateUpdate -= OnUpdateWeaponState;
    }

    public void OnUpdateWeaponState(string weaponToActivate, bool stateToSwitchTo)
    {
        for (int i = 1; i < weaponList.Count; i++)
        {
            if (weaponList[i].name == weaponToActivate)
            {
                weaponList[i].SetActive(stateToSwitchTo);
            }
        }
    }
}

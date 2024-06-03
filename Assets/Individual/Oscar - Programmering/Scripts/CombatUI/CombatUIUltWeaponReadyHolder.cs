using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class CombatUIUltWeaponReadyHolder : MonoBehaviour
{
    public WeaponType currentWeaponType;
    //public Image imageTarget;
    //public GameObject objectToSet;
    public VisualEffect vfx;

    public bool shouldPlay;
    
    public void PlaySystem()
    {
       
        vfx.SendEvent("OnPlay");
        
        
    }
    
    public void StopSystem()
    {
        vfx.SendEvent("OnStop");
    }
}

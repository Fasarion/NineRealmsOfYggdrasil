using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Patrik;

[CreateAssetMenu(menuName = "ScriptableObjects/Audio/WeaponAudio")]
public class WeaponAudio : ScriptableObject
{
    private int value = 0;
    [Header("Weapons")] 
    public EventReference swordAudio;

    public EventReference hammerAudio;
    
    public EventInstance SwordInstance;
    public EventInstance HammerInstance;

    public void WeaponSwingAudio(int weapon, int attackType)
    {
        switch (weapon)
        {
            case 0:
            {
                SwordInstance = RuntimeManager.CreateInstance(swordAudio);
                SwordInstance.setParameterByName("AttackType", attackType);
                break;
            }
        }
        /*WepInstance = RuntimeManager.CreateInstance(weaponAudio);
        //RuntimeManager.AttachInstanceToGameObject(gameObject);
        WepInstance.setParameterByName("WeaponType", weapon);
        WepInstance.setParameterByName("AttackType", attackType);
        WepInstance.start();
        WepInstance.release();*/
    }
    public void Test()
    {
       
    }
}
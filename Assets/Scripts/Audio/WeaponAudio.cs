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
    public EventReference weaponAudio;

    public EventReference testRef;
    public EventInstance WepInstance;
    public EventReference testTwo;

    public void WeaponSwingAudio(int weapon, int attackType)
    {
        /*WepInstance = RuntimeManager.CreateInstance(weaponAudio);
        //RuntimeManager.AttachInstanceToGameObject(gameObject);
        WepInstance.setParameterByName("WeaponType", weapon);
        WepInstance.setParameterByName("AttackType", attackType);
        WepInstance.start();
        WepInstance.release();*/
        Debug.Log("AUDIO AUDIO WEEWOO");
    }
    public void Test()
    {
       
    }
}
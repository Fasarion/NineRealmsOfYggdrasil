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

    public EventReference meadAudio;
    public EventReference crowAudio;

    private EventInstance _swordInstance;
    private EventInstance _hammerInstance;
    private EventInstance _meadInstance;
    private EventInstance _crowInstance;
    public void WeaponSwingAudio(int weapon, int attackType)
    {
        switch (weapon)
        {
            
            case 1:
            {
                _swordInstance = RuntimeManager.CreateInstance(swordAudio);
                _swordInstance.setParameterByName("AttackType", attackType);
                _swordInstance.start();
                _swordInstance.release();
                break;
            }
            case 2:
            {
                _hammerInstance = RuntimeManager.CreateInstance(hammerAudio);
                _hammerInstance.setParameterByName("AttackType", attackType);
                _hammerInstance.start();
                _hammerInstance.release();
                break;
            }
            case 3:
            {
                _meadInstance = RuntimeManager.CreateInstance(meadAudio);
                _meadInstance.setParameterByName("AttackType", attackType);
                _meadInstance.start();
                _meadInstance.release();
                break;
            }
            case 4:
            {
                _crowInstance = RuntimeManager.CreateInstance(crowAudio);
                _crowInstance.setParameterByName("AttackType", attackType);
                _crowInstance.start();
                _crowInstance.release();
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
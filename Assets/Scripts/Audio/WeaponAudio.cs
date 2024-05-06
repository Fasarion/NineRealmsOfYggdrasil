using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Patrik;
using Unity.VisualScripting;

[CreateAssetMenu(menuName = "ScriptableObjects/Audio/WeaponAudio")]
public class WeaponAudio : ScriptableObject
{
    private int value = 0;
    [Header("Sword")] 
    public EventReference swordSwing;
    public EventReference swordImpact;
    public EventReference iceChargeAttack;
    private EventInstance _swordInstance;
    private EventInstance _chargeInstance;
    
    [Header("Hammer")]
    public EventReference hammerSwing;
    public EventReference hammerImpact;
    public EventReference lightningStrike;
    private EventInstance _hammerInstance;
    
    [Header("Mead")]
    public EventReference meadAudio;
    private EventInstance _meadInstance;
    
    [Header("Crow")]
    public EventReference crowAudio;
    private EventInstance _crowInstance;
    
    
    
    public void WeaponSwingAudio(int weapon, int attackType)
    {
        switch (weapon)
        {
            
            case 1:
            {
                RuntimeManager.PlayOneShot(swordSwing);
                break;
            }
            case 2:
            {
                RuntimeManager.PlayOneShot(hammerSwing);
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
    }

    public void LightningStrikeAudio()
    {
        RuntimeManager.PlayOneShot(lightningStrike);
    }
    //Kallar på ljud när vapen träffar något
    public void WeaponAudioCaller(int weapon)
    {
        switch (weapon)
        {
            case 1:
            {
                RuntimeManager.PlayOneShot(swordImpact);
                //Debug.Log("hur många?");
                break;
            }
            case 2:
            {
                RuntimeManager.PlayOneShot(hammerImpact);
                break;
            }
        }
    }
    public void ChargeAttackAudio(int chargeLevel)
    {
        _chargeInstance = RuntimeManager.CreateInstance(iceChargeAttack);
        _chargeInstance.setParameterByName("ChargeLevel", chargeLevel);
        _chargeInstance.start();
        _chargeInstance.release();
    }
}
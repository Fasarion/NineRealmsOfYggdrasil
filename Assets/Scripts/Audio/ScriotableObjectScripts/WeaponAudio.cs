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
    public EventReference iceChargeImpact;
    public EventReference swordUltimate;
    private EventInstance _swordInstance;
    private EventInstance _chargeInstance;
    
    [Header("Hammer")]
    public EventReference hammerSwing;
    public EventReference hammerImpact;
    public EventReference hammerThrow;
    public EventReference hammerThrowImpact;
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
                RuntimeManager.PlayOneShot(crowAudio);
                break;
            }
            case 4:
            {
                RuntimeManager.PlayOneShot(meadAudio);
                break;
            }
        }
    }

    public void LightningStrikeAudio()
    {
        RuntimeManager.PlayOneShot(lightningStrike);
    }
    
    //Directs data to correct attack type (normal, special, ultimate)
    public void WeaponAudioCaller(int attackType, AudioData audioData)
    {
        switch (attackType)
        {
            case 1: //Normal Attacks
            {
                NormalAttackAudio((int)audioData.audioEventType, audioData);
                break;
            }
            case 2: //Special Attacks (rightclick)
            {
                SpecialAttackAudio((int)audioData.audioEventType, audioData);
                break;
            }
            case 3: //Ultimate Attacks (R)
            {
                UltimateAttackAudio((int)audioData.audioEventType, audioData);
                break;
            }
        }
    }
    
    //Directs Normal data to correct event (use, impact, charge)
    private void NormalAttackAudio(int audioEvent, AudioData audioData)
    {
        switch (audioEvent)
        {
            case 1: 
            {
                //NotInUse, handled elsewhere
                break;
            }
            case 2:
            {
                PlayNormalImpactAudio((int)audioData.weaponType);
                break;
            }
        }
    }

    private void SpecialAttackAudio(int audioEvent, AudioData audioData)
    {
        switch (audioEvent)
        {
            case 1: 
            {
                PlaySpecialUseAudio((int)audioData.weaponType);
                break;
            }
            case 2:
            {
                PlaySpecialImpactAudio((int)audioData.weaponType);
                break;
            }
        }
    }

    private void UltimateAttackAudio(int audioEvent, AudioData audioData)
    {
        switch (audioEvent)
        {
            case 1:
            {
                //PlayUltimateUseAudio((int)audioData.weaponType);
                break;
            }
            case 2:
            {
                PlayUltimateImpactAudio((int)audioData.weaponType);
                break;
            }
        }
    }
    
    //plays impact audio for correct weapon
    private void PlayNormalImpactAudio(int weapon)
    {
        switch (weapon)
        {
            case 1: //Svärd
            {
                RuntimeManager.PlayOneShot(swordImpact);
                break;
            }
            case 2: //Hammare
            {
                RuntimeManager.PlayOneShot(hammerImpact);
                break;
            }
        }
    }

    private void PlaySpecialUseAudio(int weapon)
    {
        switch (weapon)
        {
            case 1: //Svärd
            {
                //NotInUse
                break;
            }
            case 2: //Hammare
            {
                RuntimeManager.PlayOneShot(hammerThrow);
                break;
            }
            case 3: //Bird
            {
                //bird special use
                break;
            }
            case 4: //Mead
            {
                //mead special use
                break;
            }
        }
    }
    
    //Plays special impact audio for correct weapon
    private void PlaySpecialImpactAudio(int weapon)
    {
        switch (weapon)
        {
            case 1: //Svärd
            {
                RuntimeManager.PlayOneShot(iceChargeImpact);
                break;
            }
            case 2: //Hammare
            {
                RuntimeManager.PlayOneShot(hammerThrowImpact);
                break;
            }
            case 3: //Bird
            {
                //bird special impact
                break;
            }
            case 4:
            {
                //mead special impact
                break;
            }
        }
    }

    private void PlayUltimateUseAudio(int weapon)
    {
        switch (weapon)
        {
            case 1:
            {
                //SvärdUltimateUse
                break;
            }
            case 2:
            {
                //hammerUltimate
                break;
            }
            case 3: //Bird
            {
                //birdultimateuse
                break;
            }
            case 4:
            {
                //meadultimateuse
                break;
            }
        }
    }
    private void PlayUltimateImpactAudio(int weapon)
    {
        switch (weapon)
        {
            case 1: //Svärd BIGSWORD
            {
                //RuntimeManager.PlayOneShot(iceChargeImpact);
                break;
            }
            case 2: //Hammare Lightning
            {
                RuntimeManager.PlayOneShot(lightningStrike);
                break;
            }
            case 3: //Bird
            {
                //birdultimateimpact
                break;
            }
            case 4:
            {
                //meadultimateimpact
                break;
            }
        }
    }
    /*public void ChargeAttackAudio(int chargeLevel)
    {
        _chargeInstance = RuntimeManager.CreateInstance(iceChargeAttack);
        _chargeInstance.setParameterByName("ChargeLevel", chargeLevel);
        _chargeInstance.start();
        _chargeInstance.release();
    }*/
}
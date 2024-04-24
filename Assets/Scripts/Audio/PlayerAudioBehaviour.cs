using System;
using Patrik;
using UnityEngine;


public class PlayerAudioBehaviour : MonoBehaviour
{
    private AudioManager _audioManager;
    private static WeaponType weaponType;

    private void Awake()
    {
        _audioManager = AudioManager.Instance;
    }

    private void OnEnable()
    {
        EventManager.OnWeaponSwitch += OnWeaponSwitch;
        
    }
    
    private void OnDisable()
    {
        EventManager.OnWeaponSwitch -= OnWeaponSwitch;
    }

    private void OnWeaponSwitch(WeaponBehaviour currentWeapon)
    {
        weaponType = currentWeapon.WeaponType;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ActiveWeapon", (int)weaponType);
        Debug.Log((int)weaponType);
    }

    public static int GetWeaponTypeAudio()
    {
        return (int)weaponType;
    }

    public void PlayWeaponSwingAudio(int weapon, int attackType)
    {
        if (_audioManager == null)
        {
            Debug.LogWarning("Missing Audio Manager, no sound will be played.");
            return;
        }
        _audioManager.weaponAudio.WeaponSwingAudio(weapon, attackType);
    }
}
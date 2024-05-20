using System;
using Patrik;
using UnityEngine;


public class PlayerAudioBehaviour : MonoBehaviour
{
    private AudioManager _audioManager;
    private static WeaponType weaponType = WeaponType.Sword;

    private void Awake()
    {
        _audioManager = AudioManager.Instance;
    }

    private void OnEnable()
    {
        EventManager.OnWeaponSwitch += OnWeaponSwitch;
        EventManager.OnPlayerHealthSet += OnPlayerHealthSet;
        
    }
    
    private void OnDisable()
    {
        EventManager.OnWeaponSwitch -= OnWeaponSwitch;
        EventManager.OnPlayerHealthSet -= OnPlayerHealthSet;
    }

    private void OnPlayerHealthSet(PlayerHealthData healthData)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("GlobalHealth", healthData.currentHealth);
        // $"Spelarn har {healthData.currentHealth} hp just nu");
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

    public void PlayFootstepAudio()
    {
        _audioManager.playerAudio.PlayFootstepAudio();
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
using System;
using System.Collections.Generic;
using Patrik;
using UnityEngine;


public class PlayerAudioBehaviour : MonoBehaviour
{
    private AudioManager _audioManager;
    private MusicManager _musicManager;
    private static WeaponType weaponType = WeaponType.Sword;

    private void Awake()
    {
        _audioManager = AudioManager.Instance;
        _musicManager = MusicManager.Instance;
    }

    private void OnEnable()
    {
        EventManager.OnWeaponSwitch += OnWeaponSwitch;
        EventManager.OnPlayerHealthSet += OnPlayerHealthSet;
        EventManager.OnActiveAttackStart += OnActiveAttackStart;
        EventManager.OnUltimatePerform += OnUltimateUsage;
    }
    
    private void OnDisable()
    {
        EventManager.OnWeaponSwitch -= OnWeaponSwitch;
        EventManager.OnPlayerHealthSet -= OnPlayerHealthSet;
        EventManager.OnActiveAttackStart -= OnActiveAttackStart;
        EventManager.OnUltimatePerform -= OnUltimateUsage;
    }

    private void OnActiveAttackStart(AttackData data)
    {
        PlayWeaponSwingAudio((int)data.WeaponType, (int)data.AttackType);
    }

    private void OnPlayerHealthSet(PlayerHealthData healthData)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("GlobalHealth", healthData.currentHealth);
        // $"Spelarn har {healthData.currentHealth} hp just nu");
    }

    private void OnWeaponSwitch(WeaponSetupData currentWeapon, List<WeaponSetupData> allWeapons)
    {
        weaponType = currentWeapon.WeaponType;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ActiveWeapon", (int)weaponType);
        Debug.Log((int)weaponType);
    }


    private void OnUltimateUsage(WeaponType currentWeapon, AttackData attackData)
    {
        int weaponInt = (int)currentWeapon;
        switch (weaponInt)
        {
            case 1:
            {
                _musicManager.SwordUltimateMusic();
                break;
            }
            case 2:
            {
                _musicManager.HammerUltimateMusic();
                break;
            }
            case 3:
            {
                _musicManager.BirdsUltimateMusic();
                break;
            }
        }
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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private WeaponAudio _weaponAudio;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayWeaponSound()
    {
        _weaponAudio.SwordSwingAudio(gameObject);
    }
}

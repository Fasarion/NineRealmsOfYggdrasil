using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private WeaponAudio weaponAudio;
    [SerializeField] private EnemyAudio enemyAudio;
    [SerializeField] private PlayerAudio playerAudio;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void PlayWeaponSound()
    {
        weaponAudio.SwordSwingAudio(gameObject);
    }
}

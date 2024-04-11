using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public WeaponAudio weaponAudio;
    public EnemyAudio enemyAudio;
    public PlayerAudio playerAudio;

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

    /*public void PlayWeaponSound(int weapon, int attackType)
    {
        weaponAudio.SwordSwingAudio(gameObject, weapon, attackType);
    }*/
    public void PlayAudioData(AudioData audioData)
    {
        //add some codes here
    }
}

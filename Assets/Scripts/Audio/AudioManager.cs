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
        switch ((int)audioData.eventType)
        {
            case 0: //No category
            {
                Debug.Log("No Category Type Set");
                break;
            }
            case 1: //PlayerCategory
            {
                //something player gets hit here
                break;
            }
            case 2:  //EnemyCategory
            {
                enemyAudio.EnemyAudioCaller((int)audioData.enemyTyping);
                break;
            }

            case 3: //EnvironmentCategory
            {
                //ENVIRONMENTAUDIO
                break;
            }
        }
        //add some codes here
    }
}

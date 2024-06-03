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
    public UIAudio uiAudio;
    public EnvironmentAudio environmentAudio;

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

    //gathers audiodata from different sources and directs them to the right event
    public void PlayAudioData(AudioData audioData)
    {
        switch ((int)audioData.eventCategoryType)
        {
            case 0: //No category
            {
                //Debug.Log("No Category Type Set");
                break;
            }
            case 1: //WeaponCategory
            {
                weaponAudio.WeaponAudioCaller((int)audioData.attackType, audioData);
                break;
                
            }
            case 2:  //PlayerCategory
            {
                playerAudio.PlayerAudioCaller((int) audioData.audioEventType);
                break;
                
            }

            case 3: //EnemyCategory
            {
                enemyAudio.EnemyAudioCaller((int)audioData.enemyTyping, audioData);
                break;
            }
            case 4:
            {
                environmentAudio.EnvironmentAudioCaller((int)audioData.environmentType, audioData);
                break;
            }
        }
        //add some codes here
    }
}

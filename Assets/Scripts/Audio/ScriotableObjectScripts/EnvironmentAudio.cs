using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Patrik;

[CreateAssetMenu(menuName = "ScriptableObjects/Audio/EnvironmentAudio")]
public class EnvironmentAudio : ScriptableObject
{
    public EventReference treeBreakAudio;
    
    public void EnvironmentAudioCaller(int environmentType, AudioData audioData)
    {
        switch (environmentType)
        {
            case 1:
            {
                TreeAudio((int)audioData.audioEventType);
                break;
            }
        }
    }

    private void TreeAudio(int eventType)
    {
        switch (eventType)
        {
            case 0:
            {
                break;
            }
            case 1: //OnUse
            {
                break;
            }
            case 2: //OnImpact
            {
                break;
            }
            case 3: //oncharge?
            {
                break;
            }
            case 4: //OnDeath
            {
                RuntimeManager.PlayOneShot(treeBreakAudio);
                break;
            }
        }
    }
}

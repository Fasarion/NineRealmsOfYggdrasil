using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

[CreateAssetMenu(fileName = "VolumeData", menuName = "ScriptableObjects/VolumeData")]
public class VolumeDataScriptableObject : ScriptableObject
{
    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;

    public void SaveVolume(float volume, VCAName name)
    {
        switch (name)
        {
            case VCAName.Master:
            {
                masterVolume = volume;
                break;
            }
            case VCAName.SFX:
            {
                sfxVolume = volume;
                break;
            }
            case VCAName.Music:
            {
                musicVolume = volume;
                break;
            }
        }
    }

    public float LoadVolume(VCAName name)
    {
        switch (name)
        {
            case VCAName.Master:
            {
                return masterVolume;
            }
            case VCAName.SFX:
            {
                return sfxVolume;
            }
            case VCAName.Music:
            {
                return musicVolume;
            }
            default:
            {
                return 0;
            }
        }

        
    }
}

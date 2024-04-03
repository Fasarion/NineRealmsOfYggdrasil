using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

[CreateAssetMenu(menuName = "ScriptableObjects/Audio/WeaponAudio")]
public class WeaponAudio : ScriptableObject
{
    [Header("Weapons")] 
    public EventReference swordSwing;

    public void SwordSwingAudio(GameObject gameObject)
    {
        RuntimeManager.PlayOneShotAttached(swordSwing.Guid, gameObject);
    }
}
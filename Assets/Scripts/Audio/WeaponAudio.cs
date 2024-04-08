using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

[CreateAssetMenu(menuName = "ScriptableObjects/Audio/WeaponAudio")]
public class WeaponAudio : ScriptableObject
{
    private int value = 0;
    [Header("Weapons")] 
    public EventReference swordSwing;

    public EventReference testRef;
    public EventInstance test;
    public EventReference testTwo;

    public void SwordSwingAudio(GameObject gameObject)
    {
        RuntimeManager.PlayOneShot(swordSwing.Guid);
    }
    public void Test()
    {
       
    }
}
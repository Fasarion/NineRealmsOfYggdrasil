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

    public EventReference testRef;
    public EventInstance test;
    public EventReference testTwo;

    public void SwordSwingAudio(GameObject gameObject)
    {
        RuntimeManager.PlayOneShotAttached(swordSwing.Guid, gameObject);
    }
    public void Test(GameObject gameObject, float value)
    {
        test = RuntimeManager.CreateInstance(testRef);
        RuntimeManager.AttachInstanceToGameObject(test, gameObject.transform);
        test.setParameterByName("TestParam", value);
        test.start();
        test.release();
    }
}
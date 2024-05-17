using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Patrik;

[CreateAssetMenu(menuName = "ScriptableObjects/Audio/UIAudio")]
public class UIAudio : ScriptableObject
{
    public EventReference menuClick;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void MenuClickAudio()
    {
        RuntimeManager.PlayOneShot(menuClick);
    }
}

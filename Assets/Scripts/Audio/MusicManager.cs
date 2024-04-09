using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : MonoBehaviour
{
    //
    public StudioEventEmitter menuMusic;
    // Start is called before the first frame update
    void Start()
    {
        menuMusic.Play();
    }

    // Update is called once per frame
    public void play(StudioEventEmitter emitter)
    {
        emitter.Play();
    }
    public void stop(StudioEventEmitter emitter)
    {
        emitter.Stop();
    }

    public void setParamater(StudioEventEmitter emitter, string parameter, float value)
    {
        emitter.SetParameter(parameter, value, false);
    }
    
    //TODO: Menu -> Battle musik event? separat script för caller av parametrar
    //detta skript kan ha metoder np
}

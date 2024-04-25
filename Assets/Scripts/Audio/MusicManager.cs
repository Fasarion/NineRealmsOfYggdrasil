using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Unity.Entities;

public class MusicManager : MonoBehaviour
{
    private int _enemyCount;
    
    //
    public StudioEventEmitter menuMusic;
    // Start is called before the first frame update
    void Start()
    {
        menuMusic.Play();
    }

    private void OnEnable()
    {
        var enemyCountSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EnemyCountReporterSystem>();
        enemyCountSystem.OnEnemyCountChanged += RecieveEnemyCountData;
    }

    private void OnDisable()
    {
        if (World.DefaultGameObjectInjectionWorld == null) return;
        var upgradeUISystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EnemyCountReporterSystem>();
        upgradeUISystem.OnEnemyCountChanged -= RecieveEnemyCountData;
    }

    private void RecieveEnemyCountData(int count)
    {
        _enemyCount = count;
    }

    // Update is called once per frame
    public void Play(StudioEventEmitter emitter)
    {
        emitter.Play();
    }
    public void Stop(StudioEventEmitter emitter)
    {
        emitter.Stop();
    }

    public void SetParameter(StudioEventEmitter emitter, string parameter, float value)
    {
        emitter.SetParameter(parameter, value, false);
    }
    
    //TODO: Menu -> Battle musik event? separat script för caller av parametrar
    //detta skript kan ha metoder np
}

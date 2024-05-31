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
    public static MusicManager Instance;
    //
    public StudioEventEmitter menuMusic;
    public StudioEventEmitter levelMusic;

    private int nextStage = 1;
    public int enemyCountStageOne;
    public int enemyCountStageTwo;
    public int enemyCountStageThree;
    private int nextStageCount;

    [SerializeField]private bool playLevelMusic;
    [SerializeField]private bool playMenuMusic;
    
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
    
    // Start is called before the first frame update
    void Start()
    {
        if (playLevelMusic)
        {
            levelMusic.Play();
        }

        if (playMenuMusic)
        {
            menuMusic.Play();
        }
      
        SetNextStageCount(nextStage);
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
        if (_enemyCount > nextStageCount)
        {
            levelMusic.SetParameter("EnemyCountStage", nextStage);
            nextStage++;
            SetNextStageCount(nextStage++);
        }
      
    }

    private void SetNextStageCount(int nextStage)
    {
        switch (nextStage)
        {
            case 1:
            {
                nextStageCount = enemyCountStageOne;
                break;
            }
            case 2:
            {
                nextStageCount = enemyCountStageTwo;
                break;
            }
            case 3:
            {
                nextStageCount = enemyCountStageThree;
                break;
            }
        }
    }

    public void ResetEnemyCountStage()
    {
        nextStage = 1;
        SetNextStageCount(nextStage);
        levelMusic.SetParameter("EnemyCountStage", 0);
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

    public void OnDestroy()
    {
        levelMusic.Stop();
        menuMusic.Stop();
    }
    //TODO: Menu -> Battle musik event? separat script för caller av parametrar
    //detta skript kan ha metoder np
}

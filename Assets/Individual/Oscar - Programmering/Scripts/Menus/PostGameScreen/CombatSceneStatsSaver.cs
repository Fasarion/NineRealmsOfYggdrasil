using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSceneStatsSaver : MonoBehaviour
{
    public PostGameStatsSO postGameStatsSo;
    [SerializeField] private bool resetStatsOnLoadingCombatScene;
    public void OnEnable()
    {
        EventManager.OnPlayerDeath += OnPlayerDeath;
        EventManager.OnObjectiveReached += OnObjectiveReached;

    }

    public void OnDisable()
    {
        EventManager.OnPlayerDeath -= OnPlayerDeath;
        EventManager.OnObjectiveReached -= OnObjectiveReached;

    }
    private void OnObjectiveReached()
    {
        postGameStatsSo.playTime = Mathf.RoundToInt(Time.time);
        
        //postGameStatsSo.enemyKills = We need some way to get the number of kills here;
        postGameStatsSo.hasWon = true;
    }

    public void OnPlayerDeath()
    {
        
        postGameStatsSo.hasWon = false;
    }
    void Start()
    {
        if (resetStatsOnLoadingCombatScene)
        {
            ResetStats();
        }
    }

    public void ResetStats()
    {
        postGameStatsSo.enemyKills = 0;
        postGameStatsSo.hasWon = false;
        postGameStatsSo.enemyKills = 0;
    }

  
}

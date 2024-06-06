using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSceneStatsSaver : MonoBehaviour
{
    public PostGameStatsSO postGameStatsSo;
    [SerializeField] private bool resetStatsOnLoadingCombatScene = true;
    public KillCounterBehaviour killCounterBehaviour;
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

        postGameStatsSo.enemyKills = killCounterBehaviour.GetKills();
        postGameStatsSo.hasWon = true;
    }

    public void OnPlayerDeath()
    {
        postGameStatsSo.playTime = Mathf.RoundToInt(Time.time);
        postGameStatsSo.enemyKills = killCounterBehaviour.GetKills();
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

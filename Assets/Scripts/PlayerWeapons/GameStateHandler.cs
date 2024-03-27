using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStateHandler", menuName = "EventHandlers/GameStateHandler" +
    "")]
public class GameStateHandler : ScriptableObject
{
    
    public event Action onGamePause, onGameResume, onGameWin, onGameLoss;
    

    public void test()
    {
        Debug.Log("Test");
    }

    public void GamePause()
    {
           onGamePause?.Invoke();
    }

    public void GameResume()
    {
        onGameResume?.Invoke();
    }

    public void GameLoss()
    {
        onGameLoss?.Invoke();
    }

    public void GameWin()
    {
        onGameWin?.Invoke();
    }
}

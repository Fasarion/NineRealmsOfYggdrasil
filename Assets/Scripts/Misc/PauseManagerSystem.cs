using Unity.Entities;
using UnityEngine;

public enum PauseType
{
    FreezeGame,
    PauseScreen
}

public partial class PauseManagerSystem : SystemBase
{
    private bool isPaused;
    private PauseType pauseType;

    protected override void OnStartRunning()
    {
        EventManager.OnPause += OnPause;
        EventManager.OnUnpause += OnUnpause;
    }

    protected override void OnStopRunning()
    {
        EventManager.OnPause -= OnPause;
        EventManager.OnUnpause -= OnUnpause;
    }

    private void OnPause(PauseType pauseType)
    {
        isPaused = true;
        this.pauseType = pauseType;
        
        var gameManager = SystemAPI.GetSingletonEntity<GameManagerSingleton>();
        
        if (EntityManager.HasComponent<GameUnpaused>(gameManager))
        {
            EntityManager.RemoveComponent<GameUnpaused>(gameManager);
        }
        
        UnityEngine.Time.timeScale = 0;
    }
    
    private void OnUnpause(PauseType pauseType)
    {
        isPaused = false;
        this.pauseType = pauseType;
        
        var gameManager = SystemAPI.GetSingletonEntity<GameManagerSingleton>();
        
        if (!EntityManager.HasComponent<GameUnpaused>(gameManager))
        {
            EntityManager.AddComponent<GameUnpaused>(gameManager);
        }
        
        UnityEngine.Time.timeScale = 1;
    }

    protected override void OnUpdate()
    {
        if (!SystemAPI.TryGetSingleton(out PauseInput pauseInput))
        {
            return;
        }

        if (pauseInput.KeyPressed)
        {
            // can't unpause during a freeze game (like for a selection screen)
            if (isPaused && pauseType == PauseType.FreezeGame) return;
            
            isPaused = !isPaused;
            
            if (isPaused)
            {
                EventManager.OnPause?.Invoke(PauseType.PauseScreen);
            }
            else
            {
                EventManager.OnUnpause?.Invoke(PauseType.PauseScreen);
            }
        }
    }
}
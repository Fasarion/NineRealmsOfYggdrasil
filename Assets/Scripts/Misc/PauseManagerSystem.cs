using Unity.Entities;
using UnityEngine;

public partial class PauseManagerSystem : SystemBase
{
    private bool hasSubsribed;

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

    private void OnPause()
    {
        var gameManager = SystemAPI.GetSingletonEntity<GameManagerSingleton>();
        
        if (EntityManager.HasComponent<GameUnpaused>(gameManager))
        {
            EntityManager.RemoveComponent<GameUnpaused>(gameManager);
        }
        
        UnityEngine.Time.timeScale = 0;
    }
    
    private void OnUnpause()
    {
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
            Debug.Log("Pause key pressed!");
            EventManager.OnPause?.Invoke();
        }
    }
}
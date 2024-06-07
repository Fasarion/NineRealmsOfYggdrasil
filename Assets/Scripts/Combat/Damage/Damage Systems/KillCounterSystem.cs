using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using SystemAPI = Unity.Entities.SystemAPI;

public partial class KillCounterSystem : SystemBase
{
    private KillCounterBehaviour _counter;
    private int _cachedKills;

    protected override void OnStartRunning()
    {
        ResetVariables();
        EventManager.OnSceneChange += OnSceneChange;
    }
    
    protected override void OnStopRunning()
    {
        EventManager.OnSceneChange -= OnSceneChange;
    }

    private void OnSceneChange(MenuButtonSelection arg0)
    {
        if (arg0 == MenuButtonSelection.Restart 
            || arg0 == MenuButtonSelection.ExitToMenu)
        {
            ResetVariables();
        }
    }

    private void ResetVariables()
    {
        _cachedKills = 0;
        _counter = null;
    }

    protected override void OnUpdate()
    {
        if (KillCounterBehaviour.Instance != null)
        {
            _counter = KillCounterBehaviour.Instance;
            _cachedKills = 0;
        }

        if (_counter == null) return;
        
        bool configExists = SystemAPI.TryGetSingleton<KillCounterSingleton>(out KillCounterSingleton killConfig);
        if (!configExists) return;
            
        if (killConfig.Value != _cachedKills)
        {
            _cachedKills = killConfig.Value;
            _counter.SetKills(_cachedKills);
        }
    }
}

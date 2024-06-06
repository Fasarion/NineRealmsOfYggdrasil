using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using SystemAPI = Unity.Entities.SystemAPI;

public partial class KillCounterSystem : SystemBase
{
    private KillCounterBehaviour _counter;
    private bool _hasCounter;
    private int _cachedKills;
    protected override void OnUpdate()
    {
        if (KillCounterBehaviour.Instance != null)
        {
            _counter = KillCounterBehaviour.Instance;
            _cachedKills = 0;
        }

        if (_counter != null)
        {
            var killConfig = SystemAPI.GetSingleton<KillCounterSingleton>();
            if (killConfig.Value != _cachedKills)
            {
                _cachedKills = killConfig.Value;
                _counter.SetKills(_cachedKills);
            }
        }
    }
}

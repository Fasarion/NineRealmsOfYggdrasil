using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(SpawnSystem))]
public partial class EnemyCountReporterSystem : SystemBase
{
    private int _cachedEnemyCount;
    public Action<int> OnEnemyCountChanged;
    protected override void OnUpdate()
    {
        bool ifSpawnerExists = SystemAPI.TryGetSingleton<SpawnConfig>(out SpawnConfig config);
        if (!ifSpawnerExists)
        {
            // No player level found";
            return;
        }

        int currentCount = config.enemyCount;
        if (_cachedEnemyCount != currentCount)
        {
            OnEnemyCountChanged?.Invoke(currentCount);
            _cachedEnemyCount = currentCount;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial class SpawningInitializerSystem : SystemBase
{
    private SpawningController _controller;
    private int _currentCheckpointIndex = 0;
    private int _cachedCheckpointIndex = int.MaxValue;
    private int _maxCheckpointIndex;
    private int _currentCheckpointTimerCutoff;
    private int[] _checkpointTimerCutoffs;
    private SpawningTimerCheckpointObject[] _checkpointDataObjects;
    
    protected override void OnUpdate()
    {
        var configExists = SystemAPI.TryGetSingletonRW<SpawnConfig>(out RefRW<SpawnConfig> config);
        if (!configExists)
        {
            // no config
            return;
        }
        
        config.ValueRW.timerTime += SystemAPI.Time.DeltaTime;

        //initialize config
        if (_controller == null)
        {
            _controller = SpawningController.Instance;

            if (_controller == null)
            {
                // No spawner exists
                return;
            }
            
            _currentCheckpointIndex = _controller.startingCheckpointIndex;
            _checkpointTimerCutoffs = _controller.spawningCheckpointTimes.ToArray();
            config.ValueRW.timerTime = _checkpointTimerCutoffs[_currentCheckpointIndex];
            
            if (_checkpointTimerCutoffs.Length - 1 <= _currentCheckpointIndex)
            {
                _currentCheckpointTimerCutoff = int.MaxValue;
            }
            else
            {
                _currentCheckpointTimerCutoff = _checkpointTimerCutoffs[_currentCheckpointIndex + 1];
            }

            _checkpointDataObjects = _controller.checkpointData.ToArray();
            config.ValueRW.maxTimerTime = _controller.maxTimerTime;
            config.ValueRW.minTimerTime = _controller.minTimerTime;
            config.ValueRW.minEnemySpawnPercent = _controller.minEnemySpawnPercent;
            config.ValueRW.maxEnemySpawnPercent = _controller.maxEnemySpawnPercent;
            config.ValueRW.innerSpawningRadius = _controller.innerSpawningRadius;
            config.ValueRW.outerSpawningRadius = _controller.outerSpawningRadius;
            _maxCheckpointIndex = _checkpointDataObjects.Length - 1;
            
            UpdateCheckpointValues(config);
            
        }

        if (config.ValueRO.timerTime >= _currentCheckpointTimerCutoff && _currentCheckpointIndex < _maxCheckpointIndex) _currentCheckpointIndex++;

        if (_currentCheckpointIndex == _cachedCheckpointIndex) return;
            
        _cachedCheckpointIndex = _currentCheckpointIndex;
        
        if (_checkpointTimerCutoffs.Length - 1 <= _currentCheckpointIndex)
        {
            _currentCheckpointTimerCutoff = int.MaxValue;
        }
        else
        {
            _currentCheckpointTimerCutoff = _checkpointTimerCutoffs[_currentCheckpointIndex + 1];
        }
        
        UpdateCheckpointValues(config);
            
            
            
        
        
    }

    private void UpdateCheckpointValues(RefRW<SpawnConfig> config)
    {
        var controlObject = _controller.checkpointData[_currentCheckpointIndex];
        config.ValueRW.targetEnemyCount = controlObject.targetEnemyCount;
        config.ValueRW.isInitialized = false;
        EnemyTypesInformation[] enemyInfo = controlObject.enemyTypesInformation.ToArray();
        var enemyPrefabsBuffer = SystemAPI.GetSingletonBuffer<EnemyEntityPrefabElement>(false);
        
        //reset values
        float totalWeight = 0;

        var enemyWeights = new float[enemyPrefabsBuffer.Length];

        foreach (var info in enemyInfo)
        {
            enemyWeights[(int)info.enemyType] += info.enemyWeight;
            totalWeight += info.enemyWeight;
        }

        for (int i = 0; i < enemyPrefabsBuffer.Length; i++)
        {
            enemyPrefabsBuffer.ElementAt(i).SpawnPercentValue = enemyWeights[i] / totalWeight;
        }
    }
}

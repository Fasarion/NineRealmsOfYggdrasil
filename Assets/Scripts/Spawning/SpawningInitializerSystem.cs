using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

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
            _currentCheckpointIndex = _controller.startingCheckpointIndex;
            _checkpointTimerCutoffs = _controller.spawningCheckpointTimes.ToArray();
            config.ValueRW.timerTime = _checkpointTimerCutoffs[_currentCheckpointIndex];
            
            if (_checkpointTimerCutoffs.Length <= _currentCheckpointIndex)
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
        
        UpdateCheckpointValues(config);
            
            
            
        
        
    }

    private void UpdateCheckpointValues(RefRW<SpawnConfig> config)
    {
        var controlObject = _controller.checkpointData[_currentCheckpointIndex];
        config.ValueRW.targetEnemyCount = controlObject.targetEnemyCount;
        EnemyTypesInformation[] enemyInfo = controlObject.enemyTypesInformation.ToArray();
        
        //--ADD NEW ENEMIES HERE--
        //reset values
        float totalWeight = 0;
        float baseEnemyWeight = 0;
        float crazyBoiEnemyWeight = 0;
        
        config.ValueRW.baseEnemyPercentage = 0;
        config.ValueRW.crazyBoiEnemyPercentage = 0;

        for (int i = 0; i < enemyInfo.Length; i++)
        {
            EnemyType et = enemyInfo[i].enemyType;
            
            switch (et)
            {
                case EnemyType.BaseEnemy:
                    baseEnemyWeight = enemyInfo[i].enemyWeight;
                    break;
                
                case EnemyType.CrazyBoiEnemy:
                    crazyBoiEnemyWeight = enemyInfo[i].enemyWeight;
                    break;
            }
        }

        totalWeight += baseEnemyWeight;
        totalWeight += crazyBoiEnemyWeight;

        config.ValueRW.baseEnemyPercentage = baseEnemyWeight / totalWeight;
        config.ValueRW.crazyBoiEnemyPercentage = crazyBoiEnemyWeight / totalWeight;
    }
}

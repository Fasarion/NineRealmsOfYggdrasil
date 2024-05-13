using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Content;
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
    private bool _isInitialized;
    private float startUpTimer;
    private SpawningTimerCheckpointStruct[] _checkpointDataList;
    
    protected override void OnUpdate()
    {
        if (startUpTimer < 1)
        {
            startUpTimer += SystemAPI.Time.DeltaTime;
            return;
        }
        
        
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

            if (!_isInitialized)
            {
                _isInitialized = true;
                var entity = SystemAPI.GetSingletonEntity<SpawnConfig>();
                EntityManager.AddComponent<SpawningEnabledComponent>(entity);
                _checkpointDataList = _controller.SpawningCheckpoints.ToArray();
            }
            
            _currentCheckpointIndex = _controller.startingCheckpointIndex;
            var array = new int[_checkpointDataList.Length];
            for (int i = 0; i < _checkpointDataList.Length; i++)
            {
                array[i] = _checkpointDataList[i].timerCutoffTime;
            }

            _checkpointTimerCutoffs = array;
            config = SystemAPI.GetSingletonRW<SpawnConfig>();
            config.ValueRW.timerTime = _checkpointTimerCutoffs[_currentCheckpointIndex];
            
            if (_checkpointTimerCutoffs.Length - 1 <= _currentCheckpointIndex)
            {
                _currentCheckpointTimerCutoff = int.MaxValue;
            }
            else
            {
                _currentCheckpointTimerCutoff = _checkpointTimerCutoffs[_currentCheckpointIndex + 1];
            }
            
            config.ValueRW.maxTimerTime = _controller.maxTimerTime;
            config.ValueRW.minTimerTime = _controller.minTimerTime;
            config.ValueRW.minEnemySpawnCount = _controller.minEnemySpawnCount;
            config.ValueRW.maxEnemySpawnCount = _controller.maxEnemySpawnCount;
            config.ValueRW.innerSpawningRadius = _controller.innerSpawningRadius;
            config.ValueRW.outerSpawningRadius = _controller.outerSpawningRadius;
            _maxCheckpointIndex = _checkpointDataList.Length - 1;
            
            UpdateCheckpointValues(config, _checkpointDataList);
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
        
        UpdateCheckpointValues(config, _checkpointDataList);
        
    }

    private void UpdateCheckpointValues(RefRW<SpawnConfig> config, SpawningTimerCheckpointStruct[] checkpointDataList)
    {
        var checkpointData = checkpointDataList[_currentCheckpointIndex];
        config.ValueRW.targetEnemyCount = checkpointData.targetEnemyCount;
        config.ValueRW.isInitialized = false;
        CheckpointDataStruct[] enemyInfo = checkpointData.CheckpointEnemyData.ToArray();
        var enemyPrefabsBuffer = SystemAPI.GetSingletonBuffer<EnemyEntityPrefabElement>(false);

        if (checkpointData.maxSpawnCount != 0)
        {
            config.ValueRW.minEnemySpawnCount = checkpointData.minSpawnCount;
            config.ValueRW.maxEnemySpawnCount = checkpointData.maxSpawnCount;
        }

        
        //reset values
        float totalWeight = 0;

        var enemyWeights = new float[enemyPrefabsBuffer.Length];

        foreach (var info in enemyInfo)
        {
            enemyWeights[(int)info.EnemyType] += info.Weight;
            totalWeight += info.Weight;
        }
        
        for (int i = 0; i < enemyPrefabsBuffer.Length; i++)
        {
            enemyPrefabsBuffer.ElementAt(i).SpawnPercentValue = enemyWeights[i] / totalWeight;
        }
    }
}

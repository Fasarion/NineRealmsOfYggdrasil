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
    private SpawningTimerCheckpointStruct[] _checkpointDataObjects;
    private bool _isInitialized;
    private float startUpTimer;
    
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
                Debug.Log("init");
            }
            var buffer = EntityManager.GetBuffer<SpawningCheckpointElement>(SystemAPI.GetSingletonEntity<SpawnConfig>());
            
            _currentCheckpointIndex = _controller.startingCheckpointIndex;
            var array = new int[buffer.Length];
            var array2 = new SpawningTimerCheckpointStruct[buffer.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                array[i] = (int)buffer[i].timerCutoff;
                array2[i] = new SpawningTimerCheckpointStruct
                {
                    targetEnemyCount = buffer[i].TargetEnemyCount,
                    enemyType = buffer[i].TypeOfEnemy,
                };
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

            _checkpointDataObjects = array2;
            config.ValueRW.maxTimerTime = _controller.maxTimerTime;
            config.ValueRW.minTimerTime = _controller.minTimerTime;
            config.ValueRW.minEnemySpawnCount = _controller.minEnemySpawnCount;
            config.ValueRW.maxEnemySpawnCount = _controller.maxEnemySpawnCount;
            config.ValueRW.innerSpawningRadius = _controller.innerSpawningRadius;
            config.ValueRW.outerSpawningRadius = _controller.outerSpawningRadius;
            _maxCheckpointIndex = _checkpointDataObjects.Length - 1;
            
            UpdateCheckpointValues(config, _checkpointDataObjects);
            
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
        
        UpdateCheckpointValues(config, _checkpointDataObjects);
        
    }

    private void UpdateCheckpointValues(RefRW<SpawnConfig> config, SpawningTimerCheckpointStruct[] checkpointData)
    {
        var controlObject = checkpointData[_currentCheckpointIndex];
        config.ValueRW.targetEnemyCount = controlObject.targetEnemyCount;
        config.ValueRW.isInitialized = false;
        //EnemyType[] enemyInfo = controlObject.enemyType.ToArray();
        var enemyPrefabsBuffer = SystemAPI.GetSingletonBuffer<EnemyEntityPrefabElement>(false);
        
        //reset values
        float totalWeight = 0;

        var enemyWeights = new float[enemyPrefabsBuffer.Length];
        // if (enemyInfo.Length > enemyPrefabsBuffer.Length)
        // {
        //     Debug.LogError("Error with the spawning setup! Double check that all enemy types are represented as " +
        //                    "prefabs in the SpawnConfig and/or ask David for help.");
        //     return;
        // }

        // foreach (var info in enemyInfo)
        // {
        //     enemyWeights[(int)info.enemyType] += info.enemyWeight;
        //     totalWeight += info.enemyWeight;
        // }

        enemyWeights[(int)controlObject.enemyType] = 1;

        totalWeight = 1;

        for (int i = 0; i < enemyPrefabsBuffer.Length; i++)
        {
            enemyPrefabsBuffer.ElementAt(i).SpawnPercentValue = enemyWeights[i] / totalWeight;
        }
    }
}

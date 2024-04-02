using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnConfigAuthoring : MonoBehaviour
{
    public float timerInitialMaxTime = 5f;
    public int timerInitialMaxInterval = 5;
    public float timerInitialDecrease = .01f;
    public float timerDecreaseRate = .01f;
    public float timerMinTime = .1f;

    public GameObject enemyPrefab;
    public float3 centerPoint;
    public float innerRadius = 5f;
    public float outerRadius = 10f;

    public int enemiesInitialMaxCount = 10;
    public int enemiesInitialMaxSpawnCount = 3;
    public int enemiesInitialMaxSpawnGrowth = 1;
    public int enemiesInitialMaxCountGrowth = 1;
    public int enemiesMaxCount = 100000;
    public int enemiesMaxSpawn = 1000;

    public bool onlySpawnOnce = false;
    [HideInInspector] public bool shouldBeSpawning = true;

    [HideInInspector] public int currentEnemySpawnCount = 0;
    [HideInInspector] public int currentEnemyMaxCount = 0;
    [HideInInspector] public float timerCurrentMaxTime = 0f;
    [HideInInspector] public float timerCurrentMaxInterval = 0f;
    [HideInInspector] public float currentTimerDecrease = 0f;

    [HideInInspector] public bool shouldSpawn = true;
    [HideInInspector] public float timerTime = 0f;
    [HideInInspector] public float maxTimerTime = 0f;
    [HideInInspector] public int currentTimerInterval = 0;
    [HideInInspector] public int currentEnemyCount = 0;
    [HideInInspector] public int currentEnemySpawnGrowth = 0;
    [HideInInspector] public int currentEnemyMaxCountGrowth = 0;

    public bool hasAnimated = true;

    public class SpawnConfigAuthoringBaker : Baker<SpawnConfigAuthoring>
    {
        public override void Bake(SpawnConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new SpawnConfig
                {
                    timerInitialMaxTime = authoring.timerInitialMaxTime,
                    timerInitialMaxInterval = authoring.timerInitialMaxInterval,
                    timerInitialDecrease = authoring.timerInitialDecrease,
                    timerDecreaseRate = authoring.timerDecreaseRate,
                    timerMinTime = authoring.timerMinTime,
                    enemyPrefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic),
                    centerPoint = authoring.centerPoint,
                    innerRadius = authoring.innerRadius,
                    outerRadius = authoring.outerRadius,
                    enemiesInitialMaxCount = authoring.enemiesInitialMaxCount,
                    enemiesInitialMaxSpawnCount = authoring.enemiesInitialMaxSpawnCount,
                    enemiesInitialMaxSpawnGrowth = authoring.enemiesInitialMaxSpawnGrowth,
                    enemiesInitialMaxCountGrowth = authoring.enemiesInitialMaxCountGrowth,
                    enemiesMaxCount = authoring.enemiesMaxCount,
                    enemiesMaxSpawn = authoring.enemiesMaxSpawn,
                    onlySpawnOnce = authoring.onlySpawnOnce,
                    shouldBeSpawning = authoring.shouldBeSpawning,
                    currentEnemySpawnCount = authoring.currentEnemySpawnCount,
                    currentEnemyMaxCount = authoring.currentEnemyMaxCount,
                    timerCurrentMaxTime = authoring.timerCurrentMaxTime,
                    timerCurrentMaxInterval = authoring.timerCurrentMaxInterval,
                    currentTimerDecrease = authoring.currentTimerDecrease,
                    shouldSpawn = authoring.shouldSpawn,
                    timerTime = authoring.timerTime,
                    maxTimerTime = authoring.maxTimerTime,
                    currentTimerInterval = authoring.currentTimerInterval,
                    currentEnemyCount = authoring.currentEnemyCount,
                    currentEnemySpawnGrowth = authoring.currentEnemySpawnGrowth,
                    currentEnemyMaxCountGrowth = authoring.currentEnemyMaxCountGrowth,
                    hasAnimated = authoring.hasAnimated
                });
        }
    }
}

public struct SpawnConfig : IComponentData
{
    public float timerInitialMaxTime;
    public int timerInitialMaxInterval;
    public float timerInitialDecrease;
    public float timerDecreaseRate;
    public float timerMinTime;
    public Entity enemyPrefab;
    public float3 centerPoint;
    public float innerRadius;
    public float outerRadius;
    public int enemiesInitialMaxCount;
    public int enemiesInitialMaxSpawnCount;
    public int enemiesInitialMaxSpawnGrowth;
    public int enemiesInitialMaxCountGrowth;
    public int enemiesMaxCount;
    public int enemiesMaxSpawn;
    public bool onlySpawnOnce;
    public bool shouldBeSpawning;
    public int currentEnemySpawnCount;
    public int currentEnemyMaxCount;
    public float timerCurrentMaxTime;
    public float timerCurrentMaxInterval;
    public float currentTimerDecrease;
    public bool shouldSpawn;
    public float timerTime;
    public float maxTimerTime;
    public int currentTimerInterval;
    public int currentEnemyCount;
    public int currentEnemySpawnGrowth;
    public int currentEnemyMaxCountGrowth;
    public bool hasAnimated;
}
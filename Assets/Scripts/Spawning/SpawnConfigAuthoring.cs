using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnConfigAuthoring : MonoBehaviour
{
    [HideInInspector] public float timerTime;
    [HideInInspector] public bool shouldSpawn;
    [HideInInspector] public float minTimerTime;
    [HideInInspector] public float maxTimerTime;
    [HideInInspector] public float currentTimerTime;

    [HideInInspector] public int enemyCount;
    [HideInInspector] public int targetEnemyCount;
    [HideInInspector] public float minEnemySpawnPercent;
    [HideInInspector] public float maxEnemySpawnPercent;

    public GameObject baseEnemyPrefab;
    public GameObject crazyBoiEnemyPrefab;

    [HideInInspector] public float baseEnemyPercentage;
    [HideInInspector] public float crazyBoiEnemyPercentage;

    [HideInInspector] public float innerSpawningRadius;
    [HideInInspector] public float outerSpawningRadius;

    [HideInInspector] public int currentActiveCheckpoint;

    [HideInInspector] public bool isInitialized;

    public class NewSpawnConfigBaker : Baker<SpawnConfigAuthoring>
    {
        public override void Bake(SpawnConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity,
                new SpawnConfig
                {
                    timerTime = authoring.timerTime,
                    shouldSpawn = authoring.shouldSpawn,
                    minTimerTime = authoring.minTimerTime,
                    maxTimerTime = authoring.maxTimerTime,
                    enemyCount = authoring.enemyCount,
                    targetEnemyCount = authoring.targetEnemyCount,
                    minEnemySpawnPercent = authoring.minEnemySpawnPercent,
                    maxEnemySpawnPercent = authoring.maxEnemySpawnPercent,
                    baseEnemyPrefab = GetEntity(authoring.baseEnemyPrefab, TransformUsageFlags.Dynamic),
                    crazyBoiEnemyPrefab = GetEntity(authoring.crazyBoiEnemyPrefab, TransformUsageFlags.Dynamic),
                    baseEnemyPercentage = authoring.baseEnemyPercentage,
                    crazyBoiEnemyPercentage = authoring.crazyBoiEnemyPercentage,
                    currentTimerTime = authoring.currentTimerTime,
                    innerSpawningRadius = authoring.innerSpawningRadius,
                    outerSpawningRadius = authoring.outerSpawningRadius,
                    isInitialized = authoring.isInitialized,
                });
        }
    }
}

public struct SpawnConfig : IComponentData
{
    public float timerTime;
    public bool shouldSpawn;
    public float minTimerTime;
    public float maxTimerTime;
    public int enemyCount;
    public int targetEnemyCount;
    public float minEnemySpawnPercent;
    public float maxEnemySpawnPercent;
    public Entity baseEnemyPrefab;
    public Entity crazyBoiEnemyPrefab;
    public float baseEnemyPercentage;
    public float crazyBoiEnemyPercentage;
    public float currentTimerTime;
    public float innerSpawningRadius;
    public float outerSpawningRadius;
    public bool isInitialized;
}

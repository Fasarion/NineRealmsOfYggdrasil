using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public enum EnemyType
{
    BaseEnemy,
    CrazyBoiEnemy,
    Grunt1,
    Grunt2,
    Grunt3,
}

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

    [HideInInspector] public float innerSpawningRadius;
    [HideInInspector] public float outerSpawningRadius;

    [HideInInspector] public int currentActiveCheckpoint;

    [HideInInspector] public bool isInitialized;

    public List<EnemyPrefabData> enemyPrefabs;

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
                    currentTimerTime = authoring.currentTimerTime,
                    innerSpawningRadius = authoring.innerSpawningRadius,
                    outerSpawningRadius = authoring.outerSpawningRadius,
                    isInitialized = authoring.isInitialized,
                });

            var buffer = AddBuffer<EnemyEntityPrefabElement>(entity);
            
            foreach (var enemyPrefab in authoring.enemyPrefabs)
            {
                buffer.Add(new EnemyEntityPrefabElement
                {
                   PrefabValue = GetEntity(enemyPrefab.prefab, TransformUsageFlags.Dynamic), 
                   TypeValue = enemyPrefab.enemyType
                });
            }
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
    public float currentTimerTime;
    public float innerSpawningRadius;
    public float outerSpawningRadius;
    public bool isInitialized;
}

public struct EnemyEntityPrefabElement : IBufferElementData
{
    public Entity PrefabValue;
    public EnemyType TypeValue;
    public float SpawnPercentValue;
}

[System.Serializable]
public struct EnemyPrefabData 
{ 
    public GameObject prefab;
   public EnemyType enemyType;
}

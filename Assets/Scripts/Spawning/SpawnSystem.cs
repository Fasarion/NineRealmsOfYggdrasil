using AI;
using Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
[UpdateAfter(typeof(SpawningInitializerSystem))]
[BurstCompile]
public partial struct SpawnSystem : ISystem
{
    private NativeArray<float> _enemyProbabilities;
    private NativeParallelHashMap<int, Entity> _enemyPrefabs;
    private float _timerCutoffPoint;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerPositionSingleton>();
        state.RequireForUpdate<SpawnConfig>();
        state.RequireForUpdate<RandomComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingletonRW<SpawnConfig>();
        RefRW<RandomComponent> random = SystemAPI.GetSingletonRW<RandomComponent>();


        if (!config.ValueRO.isInitialized)
        {
            _enemyProbabilities = new NativeArray<float>(2, Allocator.Persistent);
            _enemyPrefabs = new NativeParallelHashMap<int, Entity>(2, Allocator.Persistent);
            _enemyPrefabs.Add(0, config.ValueRO.baseEnemyPrefab);
            _enemyPrefabs.Add(1, config.ValueRO.crazyBoiEnemyPrefab);
            config.ValueRW.isInitialized = true;
            _timerCutoffPoint = config.ValueRO.minTimerTime;
        }
        

        config.ValueRW.currentTimerTime += SystemAPI.Time.DeltaTime;
        var currentTimerTime = config.ValueRO.currentTimerTime;
        if (currentTimerTime < _timerCutoffPoint) return;

        var query = SystemAPI.QueryBuilder().WithAll<EnemyTag, LocalTransform>().Build();
        int currentEnemyCount = query.CalculateEntityCount();
        
        config.ValueRW.currentTimerTime = 0f;
        _timerCutoffPoint = GetTimerCutoff(config, currentEnemyCount);

        
        int spawnCount = GetSpawnCount(config, currentEnemyCount);
        
        var enemySpawnTypes = new NativeArray<int>(spawnCount, Allocator.TempJob);
        
        GenerateEnemyTypesArray(random, config, spawnCount, ref enemySpawnTypes);
        
        SpawnEnemies(random, spawnCount, ref enemySpawnTypes, config, state);

        enemySpawnTypes.Dispose();
    }

    public void OnDestroy(ref SystemState state)
    {
        _enemyProbabilities.Dispose();
    }
    
    //TODO: make into job?
    [BurstCompile]
    private void SpawnEnemies(RefRW<RandomComponent> random, int spawnCount, ref NativeArray<int> enemySpawnTypes, RefRW<SpawnConfig> config, SystemState state)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            float theta = random.ValueRW.random.NextFloat(0f, math.PI * 2);
            float randomRadius = random.ValueRW.random.NextFloat(config.ValueRO.innerSpawningRadius, config.ValueRO.outerSpawningRadius);

            var playerPosition = SystemAPI.GetSingleton<PlayerPositionSingleton>().Value;
            float3 spawnPosition = GetRandomSpawnPoint(playerPosition, theta, randomRadius);
            int enemyTypeIndex = enemySpawnTypes[i];
            var enemyPrefab = GetEnemyPrefabType(enemyTypeIndex);

            // Instantiate enemy at the spawn position
            var enemy = state.EntityManager.Instantiate(enemyPrefab);
            
            state.EntityManager.SetComponentData(enemy, new LocalTransform
            {
                Position = new float3(spawnPosition.x, 0, spawnPosition.z),
                Rotation = quaternion.identity,
                Scale = 1
            });
        }
    }

    private Entity GetEnemyPrefabType(int enemyTypeIndex)
    {
        Entity prefab = _enemyPrefabs[enemyTypeIndex];
        return prefab;
    }
    
    //TODO: make into job
    [BurstCompile]
    private float3 GetRandomSpawnPoint(float3 center, float theta, float randomRadius)
    {
        // Calculate x and z coordinates based on the angle and radius
        float x = center.x + randomRadius * math.cos(theta);
        float z = center.z + randomRadius * math.sin(theta);

        // Offset the y-coordinate by the center point's y-coordinate
        float y = center.y;

        // Return the spawn position
        return new float3(x, y, z);
    }

    //TODO: make into job
    [BurstCompile]
    private void GenerateEnemyTypesArray(RefRW<RandomComponent> random, RefRW<SpawnConfig> config, int spawnCount, ref NativeArray<int> enemySpawnTypes)
    {
        _enemyProbabilities[0] = config.ValueRO.baseEnemyPercentage;
        _enemyProbabilities[1] = config.ValueRO.crazyBoiEnemyPercentage;

        for (int i = 0; i < spawnCount; i++)
        {
            float randomValue = random.ValueRW.random.NextFloat(0, 100) * 0.01f;
            float cumulativeProbability = 0f;
            
            for (int j = 0; j < _enemyProbabilities.Length; j++)
            {
                cumulativeProbability += _enemyProbabilities[j];
                if (randomValue <= cumulativeProbability)
                {
                    enemySpawnTypes[i] = j;
                }
            }
        }
    }

    [BurstCompile]
    private int GetSpawnCount(RefRW<SpawnConfig> config, int currentEnemyCount)
    {
        int minSpawnCount = (int)(config.ValueRO.targetEnemyCount * config.ValueRO.minEnemySpawnPercent);
        int maxSpawnCount = (int)(config.ValueRO.targetEnemyCount * config.ValueRO.maxEnemySpawnPercent);
        float currentPercentagePoint = ((float)currentEnemyCount / config.ValueRO.targetEnemyCount);
        float result = math.lerp(maxSpawnCount, minSpawnCount, currentPercentagePoint);
        return (int)result;
    }

    [BurstCompile]
    private float GetTimerCutoff(RefRW<SpawnConfig> config, int currentEnemyCount)
    {
        float minTimerTime =  config.ValueRO.minTimerTime;
        float maxTimerTime = config.ValueRO.maxTimerTime;
        float currentPercentagePoint = ((float)currentEnemyCount / config.ValueRO.targetEnemyCount);
        float result = math.lerp(minTimerTime, maxTimerTime, currentPercentagePoint);
        return result;
    }
}

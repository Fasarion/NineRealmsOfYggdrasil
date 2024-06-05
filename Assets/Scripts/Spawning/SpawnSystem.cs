using System.Linq;
using AI;
using Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
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
    private float _mapXMinMax;
    private float _mapZMinMax;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerPositionSingleton>();
        state.RequireForUpdate<SpawnConfig>();
        state.RequireForUpdate<RandomComponent>();
        state.RequireForUpdate<SpawningEnabledComponent>();
        _mapXMinMax = 125f;
        _mapZMinMax = 125f;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingletonRW<SpawnConfig>();
        RefRW<RandomComponent> random = SystemAPI.GetSingletonRW<RandomComponent>();
        var enemyPrefabsBuffer = SystemAPI.GetSingletonBuffer<EnemyEntityPrefabElement>(false);
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>();
        var timer = SystemAPI.GetComponentRW<TimerObject>(SystemAPI.GetSingletonEntity<SpawnConfig>());

        
        timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;
        
        //Debug.Log($"timer time {timer.ValueRO.currentTime}");

        if (timer.ValueRO.currentTime >= 1)
        {
            timer.ValueRW.currentTime = 0;
            CheckForOutOfBoundsEnemies(ref state, config.ValueRO.maxDistanceFromPlayer, playerPos.Value, config.ValueRO.outerSpawningRadius);
            //Debug.Log("check");
        }
        
        if (!config.ValueRO.isInitialized)
        {
            _enemyProbabilities = new NativeArray<float>(enemyPrefabsBuffer.Length, Allocator.Persistent);
            _enemyPrefabs = new NativeParallelHashMap<int, Entity>(enemyPrefabsBuffer.Length, Allocator.Persistent);

            for (int i = 0; i < enemyPrefabsBuffer.Length; i++)
            {
                _enemyProbabilities[i] = enemyPrefabsBuffer.ElementAt(i).SpawnPercentValue;
            }
            
            foreach (var enemyPrefab in enemyPrefabsBuffer)
            {
                if (!_enemyPrefabs.ContainsKey((int)enemyPrefab.TypeValue))
                {
                    _enemyPrefabs.Add((int)enemyPrefab.TypeValue, enemyPrefab.PrefabValue);
                }
            }
            
            config.ValueRW.isInitialized = true;
            _timerCutoffPoint = config.ValueRO.minTimerTime;
        }
        
        config.ValueRW.currentTimerTime += SystemAPI.Time.DeltaTime;
        var currentTimerTime = config.ValueRO.currentTimerTime;
        if (currentTimerTime < _timerCutoffPoint) return;

        var query = SystemAPI.QueryBuilder().WithAll<EnemyTypeComponent, LocalTransform>().Build();
        int currentEnemyCount = query.CalculateEntityCount();
        config.ValueRW.enemyCount = currentEnemyCount;
        
        config.ValueRW.currentTimerTime = 0f;
        _timerCutoffPoint = GetTimerCutoff(config, currentEnemyCount);
        
        int spawnCount = GetSpawnCount(config, currentEnemyCount);
        if (spawnCount <= 0) return;
        var enemySpawnTypes = new NativeArray<int>(spawnCount, Allocator.TempJob);
        
        GenerateEnemyTypesArray(random, config, spawnCount, ref enemySpawnTypes);
        
        SpawnEnemies(random, spawnCount, ref enemySpawnTypes, config, state);
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

        if (!_enemyPrefabs.ContainsKey(enemyTypeIndex))
        {
            Debug.LogError("Error with the spawning setup! Double check that all enemy types are represented as " +
                           "prefabs in the SpawnConfig and/or ask David for help.");
            foreach (var enemyPrefab in _enemyPrefabs)
            {
                Entity defaultPrefab = enemyPrefab.Value;
                return defaultPrefab;
            }
        }
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
                    break;
                }
            }
        }
    }

    [BurstCompile]
    private void CheckForOutOfBoundsEnemies(ref SystemState state, float maxDistance, float3 playerPos, float outerRadius)
    {
        foreach (var (transform, velocity) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<PhysicsVelocity>>()
                     .WithAll<EnemyTypeComponent>())
        {
            float distance = ((Vector3)transform.ValueRO.Position - (Vector3)playerPos).magnitude;
            if (distance > maxDistance)
            {
                transform.ValueRW.Position = (math.normalizesafe(playerPos, transform.ValueRO.Position) * outerRadius) + playerPos;
                velocity.ValueRW.Linear = new float3(0, 0, 0);
            }
            if (transform.ValueRO.Position.x > _mapXMinMax || transform.ValueRO.Position.x < -_mapXMinMax ||
                transform.ValueRO.Position.z > _mapZMinMax || transform.ValueRO.Position.z < -_mapZMinMax)
            {
                transform.ValueRW.Position = new float3(0, 1, 0);
                velocity.ValueRW.Linear = new float3(0, 0, 0);
            }
        }
    }

    [BurstCompile]
    private int GetSpawnCount(RefRW<SpawnConfig> config, int currentEnemyCount)
    {
        //int minSpawnCount = (int)math.ceil(config.ValueRO.targetEnemyCount * config.ValueRO.minEnemySpawnCount);
        //int maxSpawnCount = (int)math.ceil(config.ValueRO.targetEnemyCount * config.ValueRO.maxEnemySpawnCount);
        int minSpawnCount = (int)config.ValueRO.minEnemySpawnCount;
        int maxSpawnCount = (int)config.ValueRO.maxEnemySpawnCount;
        
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

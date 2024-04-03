using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateBefore(typeof(AnimationTestSystem))]
[BurstCompile]
public partial struct SpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingletonRW<SpawnConfig>();
        
        if (config.ValueRO.shouldSpawn)
        {
            InitializeSpawnConfig(config);
            config.ValueRW.shouldSpawn = false;
        }
        
        if(!config.ValueRO.shouldBeSpawning) return;


        
        
        config.ValueRW.timerTime += SystemAPI.Time.DeltaTime;
        var timerTime = config.ValueRO.timerTime;
        var maxTimerTime = config.ValueRO.maxTimerTime;
        
        var timerInterval = config.ValueRO.currentTimerInterval;
        var maxInterval = config.ValueRO.timerInitialMaxInterval;

        if (timerTime < maxTimerTime) return;
        
        if (config.ValueRO.onlySpawnOnce) config.ValueRW.shouldBeSpawning = false;
        SpawnEnemies(config, state);
        
        

        config.ValueRW.currentTimerInterval++;
        
        

        if (timerInterval < maxInterval) return;
        
        UpdateSpawnValues(config);

    }

    [BurstCompile]
    private void InitializeSpawnConfig(RefRW<SpawnConfig> config)
    {
        config.ValueRW.timerTime = 0f;
        config.ValueRW.maxTimerTime = config.ValueRO.timerInitialMaxTime;
        config.ValueRW.shouldSpawn = true;
        config.ValueRW.currentEnemySpawnCount = config.ValueRO.enemiesInitialMaxSpawnCount;
        config.ValueRW.currentEnemyMaxCount = config.ValueRO.enemiesInitialMaxCount;
        config.ValueRW.timerCurrentMaxTime = config.ValueRO.timerInitialMaxTime;
        config.ValueRW.timerCurrentMaxInterval = config.ValueRO.timerInitialMaxInterval;
        config.ValueRW.currentTimerDecrease = config.ValueRO.timerInitialDecrease;
        config.ValueRW.currentTimerInterval = 0;
        config.ValueRW.currentEnemySpawnGrowth = config.ValueRO.enemiesInitialMaxSpawnGrowth;
        config.ValueRW.currentEnemyMaxCountGrowth = config.ValueRO.enemiesInitialMaxSpawnGrowth;
        config.ValueRW.shouldBeSpawning = true;
        
    }

    [BurstCompile]
    private void SpawnEnemies(RefRW<SpawnConfig> config, SystemState state)
    {
        var spawnCount = config.ValueRO.currentEnemySpawnCount;
        var enemyCount = config.ValueRO.currentEnemyCount;
        var maxCount = config.ValueRO.currentEnemyMaxCount;
        var innerRadius = config.ValueRO.innerRadius;
        var outerRadius = config.ValueRO.outerRadius;
        var centerPoint = config.ValueRO.centerPoint;
        
        RefRW<RandomComponent> random = SystemAPI.GetSingletonRW<RandomComponent>();
        

        if (spawnCount + enemyCount > maxCount) spawnCount = maxCount;
        
        for (int i = 0; i < spawnCount; i++)
        {
            float theta = random.ValueRW.random.NextFloat(0f, math.PI * 2);
            float randomRadius = random.ValueRW.random.NextFloat(innerRadius, outerRadius);
            
            float3 spawnPosition = GetRandomSpawnPoint(centerPoint, theta, randomRadius);

            // Instantiate enemy at the spawn position
            var enemy = state.EntityManager.Instantiate(config.ValueRO.enemyPrefab);
            
            state.EntityManager.SetComponentData(enemy, new LocalTransform
            {
                Position = new float3(spawnPosition.x, .5f, spawnPosition.z),
                Rotation = quaternion.identity,
                Scale = 1
            });
        }
    }

    [BurstCompile]
    private void UpdateSpawnValues(RefRW<SpawnConfig> config)
    {
        config.ValueRW.timerTime = 0f;
        config.ValueRW.timerCurrentMaxTime -= (config.ValueRO.currentTimerDecrease + config.ValueRO.timerDecreaseRate);
        config.ValueRW.currentEnemySpawnGrowth += config.ValueRO.enemiesInitialMaxSpawnGrowth;
        config.ValueRW.currentEnemyMaxCountGrowth += config.ValueRO.enemiesInitialMaxSpawnGrowth;
        config.ValueRW.currentEnemySpawnCount += config.ValueRO.currentEnemySpawnGrowth;
        config.ValueRW.currentEnemyMaxCount += config.ValueRO.currentEnemyMaxCountGrowth;
        config.ValueRW.currentTimerInterval = 0;
    }

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
    
}

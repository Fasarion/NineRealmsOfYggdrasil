using System.Collections;
using System.Collections.Generic;
using AI;
using Destruction;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(ObjectiveObjectMarkerMoverSystem))]
[BurstCompile]
public partial struct PlayerObjectiveSystem : ISystem
{
    private float _spawnTimer;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ObjectiveObjectConfig>();
        _spawnTimer = 0;
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _spawnTimer += SystemAPI.Time.DeltaTime;
        var config = SystemAPI.GetSingleton<ObjectiveObjectConfig>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        if (_spawnTimer < config.SpawnRate) return;
        
        foreach (var (tag, transform, entity) in
                 SystemAPI.Query<EnemyTypeComponent, LocalTransform>().WithEntityAccess().WithNone<ObjectiveObjectShouldDropOnDeathMarkerTag>())
        {
            var buffer = state.EntityManager.GetBuffer<SpawnEntityOnDestroyElement>(entity);
            var spawnElement = new SpawnEntityOnDestroyElement
            {
                Entity = config.ObjectiveObjectPrefab,
            };
            buffer.Add(spawnElement);

            var marker = state.EntityManager.Instantiate(config.ObjectiveObjectMarkerPrefab);
            state.EntityManager.SetComponentData(marker, new ObjectiveObjectMarkerComponent{EntityToFollow = entity, Offset = config.MarkerOffset});
            
            ecb.AddComponent<ObjectiveObjectShouldDropOnDeathMarkerTag>(entity);
            _spawnTimer = 0;
            break;
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

[UpdateBefore(typeof(DestroyEntitiesSystem))]
public partial struct ObjectiveObjectMarkerMoverSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ObjectiveObjectMarkerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
        foreach (var (marker, transform, markerEntity) in
                 SystemAPI.Query<RefRW<ObjectiveObjectMarkerComponent>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            if (!state.EntityManager.Exists(marker.ValueRO.EntityToFollow))
            {
                ecb.AddComponent<ShouldBeDestroyed>(markerEntity);
            }


            if (state.EntityManager.HasComponent<LocalTransform>(marker.ValueRO.EntityToFollow))
            {
                var entityToFollowTransform = state.EntityManager.GetComponentData<LocalTransform>(marker.ValueRO.EntityToFollow);
                var entityToFollowPos = entityToFollowTransform.Position;
                transform.ValueRW.Position = entityToFollowPos + new float3(0, marker.ValueRO.Offset, 0);
            }
            
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
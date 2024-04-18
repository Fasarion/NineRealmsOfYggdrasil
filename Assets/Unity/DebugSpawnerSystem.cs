using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct DebugSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DebugSpawnerConfig>();
    }

    public void OnUpdate(ref SystemState state)
    {

        foreach (var (config, entity2) in
                 SystemAPI.Query<RefRW<DebugSpawnerConfig>>()
                     .WithEntityAccess().WithNone<ChargeTimer>())
        {
            if (config.ValueRO.spawn)
            {
                for (int i = 0; i < config.ValueRO.numberOfObjectsToSpawnSquare; i++)
                {
                    for (int j = 0; j < config.ValueRO.numberOfObjectsToSpawnSquare; j++)
                    {
                        var entity = state.EntityManager.Instantiate(config.ValueRO.objectToSpawn);
                        state.EntityManager.SetComponentData(entity, new LocalTransform
                        {
                            Position = new float3(i + 5, 0, j + 5) + config.ValueRO.spawnPointStart,
                            Rotation = quaternion.identity,
                            Scale = 1,
                        });
                    }
                }

                config.ValueRW.spawn = false;
            }
        }



        


    }


}

using System.Collections;
using System.Collections.Generic;
using Destruction;
using Movement;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
public partial struct XPObjectSpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<XPObjectConfig>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        
        var config = SystemAPI.GetSingleton<XPObjectConfig>();

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                var xpObject = state.EntityManager.Instantiate(config.xPObjectPrefab);
                // state.EntityManager.AddComponent<DirectionComponent>(xpObject);
                // state.EntityManager.AddComponent<ShouldBeDestroyed>(xpObject);
                // state.EntityManager.SetComponentEnabled<DirectionComponent>(xpObject, false);
                // state.EntityManager.SetComponentEnabled<ShouldBeDestroyed>(xpObject, false);
                state.EntityManager.SetComponentData(xpObject, new LocalTransform
                {
                    Position = new float3(i, .5f, j),
                    Rotation = Quaternion.identity,
                    Scale = 1
                });
            }
        }
    }
}

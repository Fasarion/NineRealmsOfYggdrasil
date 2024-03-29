using System.Collections;
using System.Collections.Generic;
using Destruction;
using Movement;
using Unity.Entities;
using UnityEngine;

public partial struct XPObjectSpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<XpObjectConfig>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<XpObjectConfig>();

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                var xpObject = state.EntityManager.Instantiate(config.xPObjectPrefab);
                state.EntityManager.AddComponent<DirectionComponent>(xpObject);
                state.EntityManager.AddComponent<ShouldBeDestroyed>(xpObject);
                state.EntityManager.SetComponentEnabled<DirectionComponent>(xpObject, false);
                state.EntityManager.SetComponentEnabled<ShouldBeDestroyed>(xpObject, false);
            }
        }
    }
}

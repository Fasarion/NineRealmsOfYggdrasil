using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial struct WinSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WinComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {

        EventManager.OnObjectiveReached();
        var component = SystemAPI.GetSingletonEntity<WinComponent>();
        state.EntityManager.DestroyEntity(component);
            

    }
}

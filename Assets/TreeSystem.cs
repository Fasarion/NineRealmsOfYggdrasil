using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TreeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (ability, entity) in
                 SystemAPI.Query<TreeComponent>()
                     .WithEntityAccess())
        {
            var buffer = state.EntityManager.GetBuffer<Child>(entity);
            Debug.Log($"{buffer.Length}");
        }
    }
}

using Patrik;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

[UpdateAfter(typeof(PlayerAttackSystem))]
public partial struct UltimateAttackTargetingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTargetingComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        bool shouldFollow = false;
        
        foreach (var (weapon, entity) in SystemAPI
            .Query<RefRW<WeaponComponent>>()
            .WithAll<ActiveWeapon>()
            .WithEntityAccess())
        {
            if (!weapon.ValueRO.UsesTargetingForUltimate) 
                break;
            
            if (!weapon.ValueRO.ShouldSelectTarget) 
                break;

            shouldFollow = true;
        }

       // if (!shouldFollow) return;

        var mousePos = SystemAPI.GetSingleton<MousePositionInput>();

        foreach (var (transform, target) in SystemAPI.Query<RefRW<LocalTransform>, PlayerTargetingComponent>())
        {
            transform.ValueRW.Position = mousePos.WorldPosition;
            
            // Debug.Log($"target pos: {transform.ValueRO.Position}");
            // Debug.Log($"mouse pos: {mousePos.WorldPosition}");
        }

    }
}
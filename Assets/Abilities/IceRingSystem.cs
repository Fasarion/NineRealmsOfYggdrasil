using System.Collections;
using System.Collections.Generic;
using Destruction;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct IceRingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<IceRingAbility>();
        state.RequireForUpdate<IceRingConfig>();
        state.RequireForUpdate<PlayerPositionSingleton>();
        state.RequireForUpdate<PlayerSpecialAttackInput>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingletonRW<IceRingConfig>();
        var input = SystemAPI.GetSingleton<PlayerSpecialAttackInput>();

        foreach (var (ability, transform, chargeTimer, entity) in
                 SystemAPI.Query<RefRW<IceRingAbility>, RefRW<LocalTransform>, RefRW<ChargeTimer>>()
                     .WithEntityAccess())
        {


            if (!input.IsHeld)
            {
                state.EntityManager.AddComponent<ShouldBeDestroyed>(entity);
                var effect = state.EntityManager.Instantiate(config.ValueRO.abilityPrefab);

                state.EntityManager.SetComponentData(effect, new LocalTransform
                {
                    Position = new float3(0, 0, 0),
                    Rotation = quaternion.identity,
                    Scale = 1
                });
            }
            


        }

        foreach (var (ability, timer, transform, entity) in
                 SystemAPI.Query<RefRW<IceRingAbility>, RefRW<TimerObject>, RefRW<LocalTransform>>()
                     .WithEntityAccess().WithNone<ChargeTimer>())
        {
            //set up
            if (!ability.ValueRO.isInitialized)
            {
                timer.ValueRW.maxTime = config.ValueRO.maxDisplayTime;
            }
            
            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;
            if (timer.ValueRO.currentTime > timer.ValueRO.maxTime)
            {
                state.EntityManager.AddComponent<ShouldBeDestroyed>(entity);
            }
        }
    }
}

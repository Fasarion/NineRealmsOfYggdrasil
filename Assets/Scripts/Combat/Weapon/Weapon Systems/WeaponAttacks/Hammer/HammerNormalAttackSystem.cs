using System.Collections;
using System.Collections.Generic;
using Damage;
using Patrik;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEditor.Timeline.Actions;
using UnityEngine;

[BurstCompile]
public partial struct HammerNormalAttackSystem : ISystem
{
    private CollisionFilter _detectionFilter;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SwordStatsTag>();
        state.RequireForUpdate<BasePlayerStatsTag>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<HammerComponent>();

        state.RequireForUpdate<PhysicsWorldSingleton>();
        _detectionFilter = new CollisionFilter
        {
            BelongsTo = 1, // Projectile
            CollidesWith = 1 << 1 // Enemy
        };
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

        if (!attackCaller.ValueRO.ShouldActiveAttackWithType(WeaponType.Hammer))
            return;

        attackCaller.ValueRW.shouldActiveAttack = false;

        //Play Audio
        var audioBuffer = SystemAPI.GetSingletonBuffer<AudioBufferData>();
        var audioData = new AudioBufferData
        {
            AudioEnumValue = 1
        };
        audioBuffer.Add(audioData);

        //CollisionCheck
        var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        var hits = new NativeList<DistanceHit>(state.WorldUpdateAllocator);

        var hammerStatsEntity = SystemAPI.GetSingletonEntity<HammerStatsTag>();
        var hammerStatsComponent = state.EntityManager.GetComponentData<WeaponStatsComponent>(hammerStatsEntity);
        
        var playerStatsEntity = SystemAPI.GetSingletonEntity<BasePlayerStatsTag>();
        var playerStatsComponent = state.EntityManager.GetComponentData<WeaponStatsComponent>(playerStatsEntity);
        
        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
        
        float totalArea = playerStatsComponent.baseArea + hammerStatsComponent.baseArea;

        foreach (var (weapon, buffer, entity) in 
                 SystemAPI.Query<WeaponComponent, DynamicBuffer<HitBufferElement>>()
                     .WithEntityAccess())
        {
            hits.Clear();
            
            
            if (collisionWorld.OverlapSphere(weapon.AttackPoint.Position, totalArea, ref hits, _detectionFilter))
            {
                foreach (var hit in hits)
                {
                    var enemyPos = transformLookup[hit.Entity].Position;
                    var colPos = hit.Position;
                    float2 directionToHit = math.normalizesafe((enemyPos.xz -  weapon.AttackPoint.Position.xz));
                    
                    //Maybe TODO: kolla om hit redan finns i buffer
                    HitBufferElement element = new HitBufferElement
                    {
                        IsHandled = false,
                        HitEntity = hit.Entity,
                        Position = colPos,
                        Normal = directionToHit

                    };
                    buffer.Add(element);
                }
            }
        }
    }
}

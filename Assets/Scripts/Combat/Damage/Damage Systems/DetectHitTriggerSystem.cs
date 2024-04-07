using System.Collections;
using System.Collections.Generic;
using Health;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;


namespace Damage
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    public partial struct DetectHitTriggerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<HpTriggerConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var hitTriggerJob = new DetectHitTriggerJob
            {
                HitBufferLookup = SystemAPI.GetBufferLookup<HitBufferElement>(),
                HitPointsLookup = SystemAPI.GetComponentLookup<CurrentHpComponent>(),
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>()
            };

            var simSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = hitTriggerJob.Schedule(simSingleton, state.Dependency);
        }
    }
    
    public struct DetectHitTriggerJob : ITriggerEventsJob
    {
        public BufferLookup<HitBufferElement> HitBufferLookup;
        [ReadOnly] public ComponentLookup<CurrentHpComponent> HitPointsLookup;
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
        
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity triggerEntity;
            Entity hitEntity;
            
            // Figure out which entity is the trigger and which entity is the hit entity
            // If there are not exactly 1 of each, this is not a valid trigger event for this case, return from job
            if (HitBufferLookup.HasBuffer(triggerEvent.EntityA) && HitPointsLookup.HasComponent(triggerEvent.EntityB))
            {
                triggerEntity = triggerEvent.EntityA;
                hitEntity = triggerEvent.EntityB;
            }
            else if (HitBufferLookup.HasBuffer(triggerEvent.EntityB) &&
                     HitPointsLookup.HasComponent(triggerEvent.EntityA))
            {
                triggerEntity = triggerEvent.EntityB;
                hitEntity = triggerEvent.EntityA;
            }
            else
            {
                return;
            }
            
            // Determine if the hit entity is already added to the trigger entity's hit buffer 
            var hitBuffer = HitBufferLookup[triggerEntity];
            foreach (var hit in hitBuffer)
            {
                if (hit.HitEntity == hitEntity) return;
            }
            
            // Need to estimate position and normal as TriggerEvent does not have these details unlike CollisionEvent
            var triggerEntityPosition = TransformLookup[triggerEntity].Position;
            var hitEntityPosition = TransformLookup[hitEntity].Position;
            
            var hitPosition = math.lerp(triggerEntityPosition, hitEntityPosition, 0.5f);
            var hitNormal = math.normalizesafe(hitEntityPosition.xz - triggerEntityPosition.xz);
            
            var newHitElement = new HitBufferElement
            {
                IsHandled = false,
                HitEntity = hitEntity,
                Position = hitPosition,
                Normal = hitNormal
            };
            
            HitBufferLookup[triggerEntity].Add(newHitElement);
        }
    }
}
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
    [DisableAutoCreation]
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    public partial struct DetectHitCollisionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var hitCollisionJob = new DetectHitCollisionJob
            {
                HitBufferLookup = SystemAPI.GetBufferLookup<HitBufferElement>(),
                
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
                InvincibilityLookup = SystemAPI.GetComponentLookup<InvincibilityComponent>(),
                
                HitColliderLookup = SystemAPI.GetComponentLookup<HitColliderComponent>(),
                HitColliderTargetLookup = SystemAPI.GetComponentLookup<HitColliderTargetComponent>(),
            };

            var simSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = hitCollisionJob.Schedule(simSingleton, state.Dependency);
        }
    }
    
    [BurstCompile]
    public struct DetectHitCollisionJob : ICollisionEventsJob
    {
        public BufferLookup<HitBufferElement> HitBufferLookup;
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
        [ReadOnly] public ComponentLookup<InvincibilityComponent> InvincibilityLookup;
        
        [ReadOnly] public ComponentLookup<HitColliderComponent> HitColliderLookup;
        [ReadOnly] public ComponentLookup<HitColliderTargetComponent> HitColliderTargetLookup;
        
        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;
            
            Entity colliderEntity;
            Entity hitEntity;
            
            // Figure out which entity is the trigger and which entity is the hit entity
            // If there are not exactly 1 of each, this is not a valid trigger event for this case, return from job
            if (HitBufferLookup.HasBuffer(entityA) && HitColliderLookup.HasComponent(entityA) && HitColliderTargetLookup.HasComponent(entityB))
            {
                colliderEntity = entityA;
                hitEntity = entityB;
            }
            else if (HitBufferLookup.HasBuffer(entityB) && HitColliderLookup.HasComponent(entityB) && HitColliderTargetLookup.HasComponent(entityA))
            {
                colliderEntity = entityB;
                hitEntity = entityA;
            }
            else
            {
                return;
            }

            // ignore collisions if entity has invincibility
            if (InvincibilityLookup.HasComponent(hitEntity) && InvincibilityLookup.IsComponentEnabled(hitEntity))
            {
                return;
            }

            // Determine if the hit entity is already added to the trigger entity's hit buffer 
            var hitBuffer = HitBufferLookup[colliderEntity];
            foreach (var hit in hitBuffer)
            {
                if (hit.HitEntity == hitEntity) return;
            }
            
            // Need to estimate position and normal as TriggerEvent does not have these details unlike CollisionEvent
            var triggerEntityPosition = TransformLookup[colliderEntity].Position;
            var hitEntityPosition = TransformLookup[hitEntity].Position;
            
            var hitPosition = math.lerp(triggerEntityPosition, hitEntityPosition, 0.5f);
            var hitNormal = math.normalizesafe(hitEntityPosition - triggerEntityPosition);
            
            var newHitElement = new HitBufferElement
            {
                IsHandled = false,
                HitEntity = hitEntity,
                Position = hitPosition,
                Normal = hitNormal
            };
            
            HitBufferLookup[colliderEntity].Add(newHitElement);
        }
    }
}

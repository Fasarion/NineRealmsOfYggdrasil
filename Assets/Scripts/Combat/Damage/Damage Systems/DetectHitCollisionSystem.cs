using Health;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Damage
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    public partial struct DetectHitCollisionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<HpTriggerConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var hitCollisionJob = new DetectHitCollisionJob
            {
                HitBufferLookup = SystemAPI.GetBufferLookup<HitBufferElement>(),
                HitPointsLookup = SystemAPI.GetComponentLookup<CurrentHpComponent>(),
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>()
            };

            var simSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = hitCollisionJob.Schedule(simSingleton, state.Dependency);
        }
    }
    
    [BurstCompile]
    public struct DetectHitCollisionJob : ICollisionEventsJob
    {
        public BufferLookup<HitBufferElement> HitBufferLookup;
        [ReadOnly] public ComponentLookup<CurrentHpComponent> HitPointsLookup;
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
        
        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;
            
            Entity colliderEntity;
            Entity hitEntity;
            
            // Figure out which entity is the trigger and which entity is the hit entity
            // If there are not exactly 1 of each, this is not a valid trigger event for this case, return from job
            if (HitBufferLookup.HasBuffer(entityA) && HitPointsLookup.HasComponent(entityB))
            {
                colliderEntity = entityA;
                hitEntity = entityB;
            }
            else if (HitBufferLookup.HasBuffer(entityB) && HitPointsLookup.HasComponent(entityA))
            {
                colliderEntity = entityB;
                hitEntity = entityA;
            }
            else
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
            var hitNormal = math.normalizesafe(hitEntityPosition.xz - triggerEntityPosition.xz);
            
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
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Movement;
using Unity.Mathematics;

namespace Movement
{
    public partial struct AutoMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (autoMove, speed, directionComp, transform) 
                in SystemAPI.Query<RefRO<AutoMoveComponent>, RefRO<MoveSpeedComponent>, RefRO<DirectionComponent>, RefRW<LocalTransform> >())
            {
                var direction = autoMove.ValueRO.MoveForward ? transform.ValueRO.Forward() : directionComp.ValueRO.Value;
                
                var targetRotation = quaternion.LookRotationSafe(direction, math.up());

                float t = autoMove.ValueRO.rotationSpeed;
                transform.ValueRW.Rotation = math.slerp(transform.ValueRO.Rotation, targetRotation, t);
                
                transform.ValueRW.Position += transform.ValueRO.Forward() * speed.ValueRO.Value * deltaTime;
            }
        }
    }
}


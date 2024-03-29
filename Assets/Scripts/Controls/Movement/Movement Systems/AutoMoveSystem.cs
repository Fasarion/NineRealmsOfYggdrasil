using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Movement;

namespace Movement
{
    public partial struct AutoMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;

            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (autoMove, speed, directionComp, transform) 
                in SystemAPI.Query<RefRO<AutoMoveComponent>, RefRO<MoveSpeedComponent>, RefRO<DirectionComponent>, RefRW<LocalTransform> >())
            {
                var direction = autoMove.ValueRO.MoveForward ? transform.ValueRO.Forward() : directionComp.ValueRO.Value;
                transform.ValueRW.Position += direction * speed.ValueRO.Value * deltaTime;
            }
        }
    }
}


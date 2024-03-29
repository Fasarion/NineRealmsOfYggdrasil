using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using Movement;

namespace Player
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    public partial struct PlayerMovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerMovementConfig>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            float ver = Input.GetAxis("Vertical");
            float hor = Input.GetAxis("Horizontal");

            if (ver == 0 && hor == 0)
                return;

            var dir = math.normalizesafe(new float3(hor, 0, ver));
            var moveVector = dir * SystemAPI.Time.DeltaTime;

            // TODO: control system
            bool applySprint = Input.GetKey(KeyCode.LeftShift);
            
            foreach (var (playerTransform, speedComp, sprintMod) 
                in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeedComponent>, RefRO<SprintComponent>>().WithAll<PlayerTag>())
            {
                float speed = speedComp.ValueRO.Value;
                if (applySprint)
                {
                    speed *= sprintMod.ValueRO.SprintModifier;
                }
                
                playerTransform.ValueRW.Position = playerTransform.ValueRO.Position + moveVector * speed;
            }
        }
    }
}
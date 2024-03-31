using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using Movement;
using Unity.Physics;

namespace Player
{
    /// <summary>
    /// System for moving a player based on its move speed and sprint modifier.
    /// </summary>
    public partial struct PlayerMovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerMovementConfig>();
        }
    
        public void OnUpdate(ref SystemState state)
        {
            float hor = Input.GetAxis("Horizontal");
            float ver = Input.GetAxis("Vertical");
    
            float3 input = new float3(hor, 0, ver);
            
            float minMagnitudeForValidMove = 0.25f;
            float magnitude = math.length(input);

            if (magnitude > 1)
                input = math.normalize(input);
            
            // TODO: control system
            bool applySprint = Input.GetKey(KeyCode.LeftShift);
            var moveVector = input * SystemAPI.Time.DeltaTime;
            var playerPosSingleton = SystemAPI.GetSingletonRW<PlayerPositionSingleton>();
            
            if (magnitude < minMagnitudeForValidMove)
                return;
            
            foreach (var (playerTransform, speedComp, sprintMod) 
                in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeedComponent>, RefRO<SprintComponent>>().WithAll<PlayerTag>())
            {
                float speed = speedComp.ValueRO.Value;
                if (applySprint)
                {
                    speed *= sprintMod.ValueRO.SprintModifier;
                }
        
                var newPos = playerTransform.ValueRO.Position +  moveVector * speed;
                playerTransform.ValueRW.Position = newPos;
                playerPosSingleton.ValueRW.Value = newPos;
            }
        }
    }
}
using Movement;
using Patrik;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Player
{
    /// <summary>
    /// System for matching entity player transform with gameobject player.
    /// </summary>
    [UpdateBefore(typeof(PlayerMovementSystem))]
    public partial struct SetUpPlayerEntitySystem : ISystem 
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerMovementConfig>();
            state.RequireForUpdate<PlayerTag>(); 
        }
    
        public void OnUpdate(ref SystemState state)
        {
            foreach (var playerTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<PlayerTag>())
            {
                if (PlayerParentBehaviour.Instance != null)
                {
                    playerTransform.ValueRW.Position = PlayerParentBehaviour.Instance.transform.position;
                    playerTransform.ValueRW.Rotation = PlayerParentBehaviour.Instance.transform.rotation;
                    
                }
            }

            state.Enabled = false;
        }
    }
}
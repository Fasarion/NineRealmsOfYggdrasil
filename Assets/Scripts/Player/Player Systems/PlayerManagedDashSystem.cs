using Destruction;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Player
{
    [UpdateAfter(typeof(PlayerMovementSystem))]
    public partial class PlayerManagedDashSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            // TODO: create a more general attach to player tag ?
            foreach (var (transform, shield) in SystemAPI.Query<RefRW<LocalTransform>, DashShieldComponent>())
            {
                var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>();
                var playerRot = SystemAPI.GetSingleton<PlayerRotationSingleton>();

                float3 playerForward = playerRot.Forward;

                transform.ValueRW.Position = playerPos.Value + playerForward * shield.OffsetFromPlayer;
            }
        }
        
        protected override void OnStartRunning()
        {
            EventManager.OnDashBegin += OnDashBegin;
            EventManager.OnDashEnd += OnDashEnd;
        }

        protected override void OnStopRunning()
        {
            EventManager.OnDashBegin -= OnDashBegin;
            EventManager.OnDashEnd -= OnDashEnd;
        }
        
        private void OnDashBegin()
        {
            if (!SystemAPI.TryGetSingletonRW(out RefRW<PlayerDashConfig> dashConfig))
            {
                Debug.LogError("No Player Dash Config exists!");
                return;
            }

            var dashShieldPrefab = dashConfig.ValueRO.DashShieldPrefab;
            if (dashShieldPrefab == Entity.Null)
            {
                Debug.LogWarning("No Dash Shield prefab assigned!");
                return;
            }

            EntityManager.Instantiate(dashShieldPrefab);

        }

        private void OnDashEnd()
        {
            if (!SystemAPI.TryGetSingletonRW(out RefRW<PlayerDashConfig> dashConfig))
            {
                Debug.LogError("No Player Dash Config exists!");
                return;
            }

            var dashShieldPrefab = dashConfig.ValueRO.DashShieldPrefab;
            if (dashShieldPrefab == Entity.Null)
            {
                Debug.LogWarning("No Dash Shield prefab assigned!");
                return;
            }

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            // TODO: create a more general attach to player tag ?
            foreach (var (shield, entity) in SystemAPI.Query<DashShieldComponent>().WithEntityAccess())
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);
            }
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}
using Destruction;
using Health;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Entities.Content;
using Unity.VisualScripting;
using UnityEngine;

namespace Damage
{
    [UpdateAfter(typeof(ApplyDamageSystem))]
    [UpdateInGroup(typeof(CombatSystemGroup))]
    [UpdateBefore(typeof(DisableHasChangedHealthTagsSystem))]
    public partial struct PlayerHealthSystem : ISystem
    {
        private bool hasInitialized;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!hasInitialized)
            {
                foreach (var (currentHP, maxHealth) in
                    SystemAPI.Query<CurrentHpComponent, MaxHpComponent>()
                        .WithAll<PlayerTag>())
                {
                    PlayerHealthData data = new PlayerHealthData
                    {
                        currentHealth = currentHP.Value,
                        maxHealth = maxHealth.Value
                    };

                    EventManager.OnPlayerHealthSet?.Invoke(data);
                }
                
                hasInitialized = true;
            }
            
            foreach (var (currentHP, maxHealth) in
                SystemAPI.Query<CurrentHpComponent, MaxHpComponent>()
                    .WithAll<PlayerTag, HasChangedHP>())
            {
                PlayerHealthData data = new PlayerHealthData
                {
                    currentHealth = currentHP.Value,
                    maxHealth = maxHealth.Value
                };

                EventManager.OnPlayerHealthSet?.Invoke(data);
            }

            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            
            foreach (var (dying, entity) in SystemAPI
                .Query<RefRW<IsDyingComponent>>()
                .WithAll<PlayerTag>()
                .WithNone<PlayerDeath>()
                .WithEntityAccess())
            {
                ecb.SetComponentEnabled<ShouldBeDestroyed>(entity, true);
                EventManager.OnPlayerDeath?.Invoke();
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
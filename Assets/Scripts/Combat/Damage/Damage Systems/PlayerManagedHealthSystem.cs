using Health;
using Player;
using Unity.Entities;
using UnityEngine;

namespace Damage
{
    public partial class PlayerManagedHealthSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            // if (Input.GetKeyDown(KeyCode.P))
            // {
            //     EventManager.OnPlayerPermanentInvincibility?.Invoke(0);
            // }
            //
            // if (Input.GetKeyDown(KeyCode.L))
            // {
            //     EventManager.OnPlayerPermanentInvincibility?.Invoke(1);
            // }
        }

        protected override void OnStartRunning()
        {
            EventManager.OnEnablePlayerInvincibility += OnEnablePlayerInvincibility;
            EventManager.OnPlayerDamageReductionSet += OnPlayerDamageReductionSet;

        }
        
        protected override void OnStopRunning()
        {
            EventManager.OnEnablePlayerInvincibility -= OnEnablePlayerInvincibility;
            EventManager.OnPlayerDamageReductionSet -= OnPlayerDamageReductionSet;
        }
        
        private void OnPlayerDamageReductionSet(float reduction)
        {
            bool playerExists = SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity player);
            if (!playerExists)
            {
                Debug.LogWarning("No player exists.");
                return;
            }
            
            var damageReduction = SystemAPI.GetComponentRW<DamageReductionComponent>(player);
            damageReduction.ValueRW.Value = reduction;
        }
        
        private void OnEnablePlayerInvincibility(bool enable)
        {
            foreach (var (currentHP, maxHealth, entity) in
                SystemAPI.Query<CurrentHpComponent, MaxHpComponent>()
                    .WithEntityAccess()
                    .WithAll<PlayerTag>())
            {
                EntityManager.SetComponentEnabled<InvincibilityComponent>(entity, enable);
                var currentInvincibility = EntityManager.GetComponentData<InvincibilityComponent>(entity);

                // sets a high invincibility time to avoid the timer running out too early
                float enabledIncincibilityTime = 1000f;
                currentInvincibility.CurrentTime = enable ? enabledIncincibilityTime : 0;
                
                EntityManager.SetComponentData(entity, currentInvincibility);
            }
        }
    }
}
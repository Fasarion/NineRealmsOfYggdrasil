using Health;
using Player;
using Unity.Entities;

namespace Damage
{
    public partial class PlayerManagedHealthSystem : SystemBase
    {
        private bool hasInitialized;
        
        protected override void OnUpdate()
        {
            if (!hasInitialized)
            {
                EventManager.OnEnablePlayerInvincibility += OnEnablePlayerInvincibility;
                hasInitialized = true;
            }
        }
        
        protected override void OnStopRunning()
        {
            EventManager.OnEnablePlayerInvincibility -= OnEnablePlayerInvincibility;
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
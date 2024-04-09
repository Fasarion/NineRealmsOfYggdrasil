using Health;
using Player;
using Unity.Entities;
using UnityEngine;

namespace Damage
{
    [UpdateInGroup(typeof(CombatSystemGroup))]
    [UpdateAfter(typeof(ApplyDamageSystem))]
    [UpdateBefore(typeof(DisableHasChangedHealthTagsSystem))]
    public partial class PlayerTakingDamageSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            // Update Invincibility Timer
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var damageTaker in 
                SystemAPI.Query<HasChangedHP>()
                    .WithAll<PlayerTag>())
            {
                if (damageTaker.Amount >= 0)
                    return;

                var damageUI = UIDamageBehaviour.Instance;
                if (damageUI)
                {
                    damageUI.FlashDamage();
                }
                
            }
        }
    }
}
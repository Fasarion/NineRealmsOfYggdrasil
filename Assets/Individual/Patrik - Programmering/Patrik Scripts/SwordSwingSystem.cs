using Unity.Entities;
using UnityEngine;

namespace Patrik
{
    public partial class SwordSwingSystem : SystemBase
    {
        private PlayerWeaponHandlerBehaviour weaponHandler;

        protected override void OnUpdate()
        {
            PlayerFireInput fireInput = SystemAPI.GetSingleton<PlayerFireInput>();
            if (!fireInput.FireKeyPressed) return;

            if (weaponHandler == null)
            {
                weaponHandler = PlayerWeaponHandlerBehaviour.Instance;

                weaponHandler.OnPerformAttack += OnAttackPerformed;
            }
            
            if (weaponHandler == null)
            {
                Debug.LogError("Missing Player Weapon Handler.");
                return;
            }
            
            weaponHandler.TryPerformCurrentAttack();
        }

        protected override void OnStopRunning()
        {
            weaponHandler.OnPerformAttack -= OnAttackPerformed;
        }

        void OnAttackPerformed()
        {
            Debug.Log("Attack event recieved in DOTS!");
        }
    }
    
}
using Damage;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Patrik
{
    public partial class PlayerAttackSystem : SystemBase
    {
        private PlayerWeaponManagerBehaviour _weaponManager;

        private bool hasSetUp;

        protected override void OnUpdate()
        {
            if (!hasSetUp)
            {
                if (_weaponManager == null)
                {
                    _weaponManager = PlayerWeaponManagerBehaviour.Instance;
                    
                    if (_weaponManager == null)
                    {
                        Debug.LogWarning("Missing Player Weapon Handler, attacks not possible.");
                        return;
                    }

                    SubscribeToAttackEvents();

                    // disable sword collider at start
                    OnNormalAttackStop(new AttackData()); 
                    hasSetUp = true;
                }
            }

            HandleWeaponInput();
        }

        private void SubscribeToAttackEvents()
        {
            _weaponManager.OnActiveWeaponStartAttackNormal += OnNormalAttackStart;
            _weaponManager.OnActiveWeaponStopAttackNormal += OnNormalAttackStop;
        }

        private void HandleWeaponInput()
        {
            // Handle normal attack
            var normalAttackInput = SystemAPI.GetSingleton<PlayerNormalAttackInput>();
            if (normalAttackInput.KeyPressed)
            {
                _weaponManager.NormalAttack();
                return;
            }

            // Handle special attack
            var specialAttack = SystemAPI.GetSingleton<PlayerSpecialAttackInput>();
            if (specialAttack.KeyPressed)
            {
                _weaponManager.SpecialAttack();
                return;
            }
            
            // Handle special attack
            var ultimateAttack = SystemAPI.GetSingleton<PlayerUltimateAttackInput>();
            if (ultimateAttack.KeyPressed)
            {
                Debug.Log("Ultimate attack!");
                //_weaponManager.UltimateAttack();
                return;
            }
        }


        protected override void OnStopRunning()
        {
            UnsubscribeFromAttackEvents();
        }

        private void UnsubscribeFromAttackEvents()
        {
            _weaponManager.OnActiveWeaponStartAttackNormal -= OnNormalAttackStart;
            _weaponManager.OnActiveWeaponStopAttackNormal -= OnNormalAttackStop;
        }

        void OnNormalAttackStart(AttackData data)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (transform, sword, entity) in SystemAPI.Query<RefRW<LocalTransform>, SwordComponent>()
                .WithAll<Disabled>()
                .WithEntityAccess())
            {
                ecb.RemoveComponent(entity, typeof(Disabled));
            }
            
            ecb.Playback(EntityManager);
        }
        
        private void OnNormalAttackStop(AttackData data)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (transform, sword, damageBuffer, entity) in 
                SystemAPI.Query<RefRW<LocalTransform>, SwordComponent, DynamicBuffer<HitBufferElement>>().WithEntityAccess())
            {
                
                ecb.AddComponent(entity, typeof(Disabled));

                damageBuffer.Clear();
            } 
            
            ecb.Playback(EntityManager);
        }
    }
}
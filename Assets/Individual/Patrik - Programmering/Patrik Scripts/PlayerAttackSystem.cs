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

                    DisableAllWeapons();
                    SubscribeToAttackEvents();
                    hasSetUp = true;
                }
            }

            HandleWeaponInput();
        }

        private void DisableAllWeapons()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var ( _, entity) in 
                SystemAPI.Query<WeaponComponent>().WithEntityAccess())
            {
                ecb.AddComponent(entity, typeof(Disabled));
            }

            ecb.Playback(EntityManager);
        }

        private void SubscribeToAttackEvents()
        {
            _weaponManager.OnActiveAttackStart += OnActiveAttackStart;
            _weaponManager.OnActiveAttackStop += OnActiveAttackStop;
            _weaponManager.OnPassiveAttackStart += OnPassiveAttackStart;
            _weaponManager.OnPassiveAttackStop += OnPassiveAttackStop;
        }

        private void OnPassiveAttackStart(AttackData arg0)
        {
            Debug.Log("Passive Start!");
        }

        private void OnPassiveAttackStop(AttackData arg0)
        {
            Debug.Log("Passive Stop!");
        }

        private void HandleWeaponInput()
        {
            // Handle normal attack
            var normalAttackInput = SystemAPI.GetSingleton<PlayerNormalAttackInput>();
            if (normalAttackInput.KeyPressed)
            {
                _weaponManager.PerformNormalAttack();
                return;
            }

            // Handle special attack
            var specialAttack = SystemAPI.GetSingleton<PlayerSpecialAttackInput>();
            if (specialAttack.KeyPressed)
            {
                _weaponManager.PerformSpecialAttack();
                return;
            }
            
            // Handle special attack
            var ultimateAttack = SystemAPI.GetSingleton<PlayerUltimateAttackInput>();
            if (ultimateAttack.KeyPressed)
            {
                Debug.Log("Trigger ultimate attack event!");
                // TODO: ult attack event in weapon manager
                // todo: energy meter
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
            _weaponManager.OnActiveAttackStart -= OnActiveAttackStart;
            _weaponManager.OnActiveAttackStop -= OnActiveAttackStop;
            
            _weaponManager.OnPassiveAttackStart -= OnPassiveAttackStart;
            _weaponManager.OnPassiveAttackStop -= OnPassiveAttackStop;
        }

        void OnActiveAttackStart(AttackData data)
        {
            switch (data.AttackType)
            {
                case AttackType.Normal:
                    StartNormalAttack(data);
                    break;
                
                case AttackType.Special:
                    StartSpecialAttack(data);
                    break;
                
                case AttackType.Ultimate:
                    StartUltimateAttack(data);
                    break;
            }
        }
        
        private void OnActiveAttackStop(AttackData data)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // handle sword attack stop
            if (data.WeaponType == WeaponType.Sword)
            {
                foreach (var (transform, sword, damageBuffer, entity) in 
                    SystemAPI.Query<RefRW<LocalTransform>, SwordComponent, DynamicBuffer<HitBufferElement>>().WithEntityAccess())
                {
                
                    ecb.AddComponent(entity, typeof(Disabled));

                    damageBuffer.Clear();
                } 
            }
            
            ecb.Playback(EntityManager);
        }

        private void StartNormalAttack(AttackData data)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            
            // sword attack start
            if (data.WeaponType == WeaponType.Sword)
            {
                foreach (var (transform, sword, entity) in SystemAPI.Query<RefRW<LocalTransform>, SwordComponent>()
                    .WithAll<Disabled>()
                    .WithEntityAccess())
                {
                    ecb.RemoveComponent(entity, typeof(Disabled));
                }
            }

            ecb.Playback(EntityManager);
        }
        
        private void StartSpecialAttack(AttackData data)
        {
            Debug.Log("Start special attack");
        }
        
        private void StartUltimateAttack(AttackData data)
        {
            Debug.Log("Start ultimate attack");
        }
    }
}
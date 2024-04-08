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

            HandleWeaponSwitch();
            HandleWeaponInput();
        }

        private void HandleWeaponSwitch()
        {
            if (SystemAPI.TryGetSingleton(out WeaponOneInput weapon1) && weapon1.KeyPressed)
            {
                _weaponManager.SwitchWeapon(1);
                return;
            }
            
            if (SystemAPI.TryGetSingleton(out WeaponTwoInput weapon2) && weapon2.KeyPressed)
            {
                _weaponManager.SwitchWeapon(2);
                return;
            }
            
            if (SystemAPI.TryGetSingleton(out WeaponThreeInput weapon3) && weapon3.KeyPressed)
            {
                _weaponManager.SwitchWeapon(3);
                return;
            }
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
        
        private void UnsubscribeFromAttackEvents()
        {
            _weaponManager.OnActiveAttackStart -= OnActiveAttackStart;
            _weaponManager.OnActiveAttackStop -= OnActiveAttackStop;
            
            _weaponManager.OnPassiveAttackStart -= OnPassiveAttackStart;
            _weaponManager.OnPassiveAttackStop -= OnPassiveAttackStop;
        }
        
        void OnActiveAttackStart(AttackData data)
        {
            EnableWeapon(data.WeaponType);
            
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
            DisableWeapon(data.WeaponType);
        }

        private void OnPassiveAttackStart(AttackData data)
        {
            EnableWeapon(data.WeaponType);
            
            switch (data.WeaponType)
            {
                case WeaponType.Hammer:
                    StartPassiveHammerAttack(data);
                    break;
            }
        }
        
        private void OnPassiveAttackStop(AttackData data)
        {
            DisableWeapon(data.WeaponType);
            
            switch (data.WeaponType)
            {
                case WeaponType.Hammer:
                    StopPassiveHammerAttack();
                    break;
            }
        }

        private void EnableWeapon(WeaponType dataWeaponType)
        {
            Entity entity = GetDisabledWeaponEntity(dataWeaponType);

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            if (EntityManager.HasComponent<Disabled>(entity))
                ecb.RemoveComponent(entity, typeof(Disabled));
            
            ecb.Playback(EntityManager);
        }
        
        private void DisableWeapon(WeaponType dataWeaponType)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            Entity entity = GetEnabledWeaponEntity(dataWeaponType);
            if (entity == default)
            {
                return;
            }

            if (EntityManager.HasBuffer<HitBufferElement>(entity))
            {
                var buffer = EntityManager.GetBuffer<HitBufferElement>(entity);
                buffer.Clear();
            }

            ecb.AddComponent(entity, typeof(Disabled));
            
            
            ecb.Playback(EntityManager);
        }
        
        private Entity GetEnabledWeaponEntity(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Sword:
                    foreach (var (sword, entity) in SystemAPI.Query<SwordComponent>()
                        .WithEntityAccess())
                    {
                        return entity;
                    }
                    break;
                
                case WeaponType.Hammer:
                    foreach (var (hammer, entity) in SystemAPI.Query<HammerComponent>()
                        .WithEntityAccess())
                    {
                        return entity;
                    }
                    break;
            }

            return default;
        }

        private Entity GetDisabledWeaponEntity(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Sword:
                    foreach (var (sword, entity) in SystemAPI.Query<SwordComponent>()
                        .WithAll<Disabled>()
                        .WithEntityAccess())
                    {
                        return entity;
                    }
                    break;
                
                case WeaponType.Hammer:
                    foreach (var (hammer, entity) in SystemAPI.Query<HammerComponent>()
                        .WithAll<Disabled>()
                        .WithEntityAccess())
                    {
                        return entity;
                    }
                    break;
            }

            return default;
        }

        private void StartPassiveHammerAttack(AttackData data)
        {
            Transform attackPoint = data.AttackPoint;
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // Sets up component for an attack next frame. The reason for waiting for the next frame is because the projectile
            // must spawn before the Transform System group to avoid being spawned at (0,0,0) for one frame.
            foreach (var ( weapon, hammer, entity) 
                in SystemAPI.Query< RefRW<WeaponComponent>, HammerComponent>().WithEntityAccess())
            {
                ecb.AddComponent(entity, typeof(DoNextFrame));

                LocalTransform transform = new LocalTransform
                {
                    Position = attackPoint.position,
                    Rotation = attackPoint.rotation,
                };

                weapon.ValueRW.AttackPoint = transform;
            }
            
            ecb.Playback(EntityManager);
        }
        
        private void StopPassiveHammerAttack()
        {
            //TODO: Add hammer passive attack stop behaviour here
        }

        private void HandleWeaponInput()
        {
            if (!_weaponManager)
            {
                Debug.LogWarning("No weapon manager found, cant read weapon inputs.");
                return;
            }
            
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
                // TODO: ult attack
                return;
            }
        }


        protected override void OnStopRunning()
        {
            UnsubscribeFromAttackEvents();
        }
        
        private void StartNormalAttack(AttackData data)
        {
        }
        
        private void StartSpecialAttack(AttackData data)
        {
        }
        
        private void StartUltimateAttack(AttackData data)
        {
        }
    }
}
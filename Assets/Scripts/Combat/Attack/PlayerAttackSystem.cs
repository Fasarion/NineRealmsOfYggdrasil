﻿using System;
using Damage;
using Health;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Collider = Unity.Physics.Collider;
using Material = Unity.Physics.Material;

namespace Patrik
{
    public partial class PlayerAttackSystem : SystemBase
    {
        private PlayerWeaponManagerBehaviour _weaponManager;

        private bool hasSetUpWeaponManager;

        private float disabledTransformScale = 0.001f;

        protected override void OnUpdate()
        {
            if (!hasSetUpWeaponManager)
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
                    hasSetUpWeaponManager = true;
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
            
            foreach (var ( transform, entity) in SystemAPI
                .Query<RefRW<LocalTransform>>()
                .WithAll<WeaponComponent>()
                .WithEntityAccess())
            {
                transform.ValueRW.Scale = disabledTransformScale;
            }

            ecb.Playback(EntityManager);
        }

        private void SubscribeToAttackEvents()
        {
            _weaponManager.OnActiveAttackStart += OnActiveAttackStart;
            _weaponManager.OnActiveAttackStop += OnActiveAttackStop;
            
            _weaponManager.OnPassiveAttackStart += OnPassiveAttackStart;
            _weaponManager.OnPassiveAttackStop += OnPassiveAttackStop;

            _weaponManager.OnWeaponActive += SetWeaponActive;
            _weaponManager.OnWeaponPassive += SetWeaponPassive;
        }
        
        private void UnsubscribeFromAttackEvents()
        {
            _weaponManager.OnActiveAttackStart -= OnActiveAttackStart;
            _weaponManager.OnActiveAttackStop -= OnActiveAttackStop;
            
            _weaponManager.OnPassiveAttackStart -= OnPassiveAttackStart;
            _weaponManager.OnPassiveAttackStop -= OnPassiveAttackStop;
            
            _weaponManager.OnWeaponActive -= SetWeaponActive;
            _weaponManager.OnWeaponPassive -= SetWeaponPassive;
        }

        private void SetWeaponActive(WeaponType type)
        {
            Entity entity = GetWeaponEntity(type);

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            if (SystemAPI.HasComponent<ActiveWeapon>(entity))
            {
                SystemAPI.SetComponentEnabled<ActiveWeapon>(entity, true);
            }
            
            ecb.Playback(EntityManager);
        }
        
        private void SetWeaponPassive(WeaponType type)
        {
            Entity entity = GetWeaponEntity(type);
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            if (SystemAPI.HasComponent<ActiveWeapon>(entity))
            {
                SystemAPI.SetComponentEnabled<ActiveWeapon>(entity, false);
            }
            
            ecb.Playback(EntityManager);
        }

        private Entity GetWeaponEntity(WeaponType type)
        {
            Entity entity = GetEnabledWeaponEntity(type);
            return entity;
        }

        void OnActiveAttackStart(AttackData data)
        {
            EnableWeapon(data.WeaponType);
            
            var weaponCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
            
            weaponCaller.ValueRW.shouldActiveAttack = true;
            weaponCaller.ValueRW.currentAttackType = data.AttackType;
            weaponCaller.ValueRW.currentWeaponType = data.WeaponType;
            
            WriteOverAttackDataToActiveWeapon(data);

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

        private void WriteOverAttackDataToActiveWeapon(AttackData data)
        {
            Transform attackPoint = data.AttackPoint;

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // Sets up component for an attack next frame. The reason for waiting for the next frame is because the projectile
            // must spawn before the Transform System group to avoid being spawned at (0,0,0) for one frame.
            foreach (var (weapon, entity)
                     in SystemAPI.Query<RefRW<WeaponComponent>>()
                         .WithEntityAccess().WithAll<ActiveWeapon>())
            {
                LocalTransform transform = new LocalTransform
                {
                    Position = attackPoint.position,
                    Rotation = attackPoint.rotation,
                };

                weapon.ValueRW.AttackPoint = transform;
            }

            ecb.Playback(EntityManager);
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
            Entity entity = GetWeaponEntity(dataWeaponType);
            SetEntityScale(entity, 1);
        }

        private void SetEntityScale(Entity entity, float scale)
        {
            var transform = EntityManager.GetComponentData<LocalTransform>(entity);
            transform.Scale = scale;
            EntityManager.SetComponentData(entity, transform);
        }

        private void DisableWeapon(WeaponType dataWeaponType)
        {
            Entity entity = GetWeaponEntity(dataWeaponType);
            SetEntityScale(entity, disabledTransformScale);

            // clear its hit buffer
            if (EntityManager.HasBuffer<HitBufferElement>(entity))
            {
                var buffer = EntityManager.GetBuffer<HitBufferElement>(entity);
                buffer.Clear();
            }
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

        //TODO: bryt ut detta till ett eget script imo
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
                // No weapon manager found, can't read weapon inputs.
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
            if (specialAttack.KeyDown)
            {
                _weaponManager.PerformSpecialAttack();
                return;
            }
            
            // Handle ultimate attack
            var ultimateAttack = SystemAPI.GetSingleton<PerformUltimateAttack>();
            if (ultimateAttack.Value == true)
            {
                _weaponManager.PerformUltimateAttack();

//                HandleUltimateAttackInput();
                return;
            }
        }

        private void HandleUltimateAttackInput()
        {
            // foreach (var (weapon, energyBar, entity) in SystemAPI
            //     .Query<RefRW<WeaponComponent>, RefRW<EnergyBarComponent>>()
            //     .WithAll<ActiveWeapon>()
            //     .WithNone<ResetEnergyTag>()
            //     .WithEntityAccess())
            // {
            //     // exit if weapon doesn't have enough energy
            //     if (!energyBar.ValueRO.IsFull) return;
            //
            //     // attack if ultimate doesn't use targeting
            //     if (!weapon.ValueRO.UsesTargetingForUltimate)
            //     {
            //         StartUltimateAttack(entity);
            //     }
            //
            //     // if it uses targeting, attack if target is selected
            //     if (weapon.ValueRO.HasSelectedTarget)
            //     {
            //         StartUltimateAttack(entity);
            //     }
            //     
            //     // else, start selecting target
            //     weapon.ValueRW.ShouldSelectTarget = true;
            //     return;
            // }
        }

        private void StartUltimateAttack(Entity entity)
        {
         //   EntityManager.SetComponentEnabled<ResetEnergyTag>(entity, true);
            _weaponManager.PerformUltimateAttack();
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
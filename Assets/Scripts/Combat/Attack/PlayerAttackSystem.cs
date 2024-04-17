using System;
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
            weaponCaller.ValueRW.currentCombo = data.ComboCounter;
            
            WriteOverAttackData(data);
        }

        private void WriteOverAttackData(AttackData data)
        {
            var entity = GetWeaponEntity(data.WeaponType);
            
            // fetch attack data
            AttackType attackType = data.AttackType;
            Transform attackPoint = data.AttackPoint;
            
            // update weapon component data
            var weapon = EntityManager.GetComponentData<WeaponComponent>(entity);
            LocalTransform transform = new LocalTransform
            {
                Position = attackPoint.position,
                Rotation = attackPoint.rotation,
            };
            weapon.AttackPoint = transform;
            EntityManager.SetComponentData(entity, weapon);
            
            // update damage data
            var weaponStatsComponent = EntityManager.GetComponentData<CombatStatsComponent>(entity);
            var playerStatsEntity = SystemAPI.GetSingletonEntity<BasePlayerStatsTag>();
            var playerStatsComponent = EntityManager.GetComponentData<CombatStatsComponent>(playerStatsEntity);
            
            int combo = data.ComboCounter;
            float totalDamage = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, attackType, CombatStatType.Damage, combo);
            float totalCritRate = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, attackType, CombatStatType.CriticalRate, combo);
            float totalCritMod = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, attackType, CombatStatType.CriticalModifier, combo);
            
            DamageContents damageContents = new DamageContents()
            {
                DamageValue = totalDamage,
                CriticalRate = totalCritRate,
                CriticalModifier = totalCritMod,
            };

            DamageOnTriggerComponent damageComp = EntityManager.GetComponentData<DamageOnTriggerComponent>(entity);
            damageComp.Value = damageContents;
            EntityManager.SetComponentData(entity, damageComp);
            
            // set knockback
            float totalKnockBack = CombatStats.GetCombinedStatValue(playerStatsComponent, weaponStatsComponent, attackType, CombatStatType.KnockBack, combo);

            KnockBackForce knockBackComp = EntityManager.GetComponentData<KnockBackForce>(entity);
            knockBackComp.Value = totalKnockBack;
            EntityManager.SetComponentData(entity, knockBackComp);
        }

        private void OnActiveAttackStop(AttackData data)
        {
            DisableWeapon(data.WeaponType);
        }

        private void OnPassiveAttackStart(AttackData data)
        {
            EnableWeapon(data.WeaponType);
            WriteOverAttackData(data);

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

        private void HandleWeaponInput()
        {
            if (!_weaponManager)
            {
                // No weapon manager found, can't read weapon inputs.
                return;
            }

            if (!SystemAPI.TryGetSingleton(out GameManagerSingleton gameManager))
                return;
            
            // Handle ultimate attack
            var ultimateAttack = SystemAPI.GetSingleton<PerformUltimateAttack>();
            if (ultimateAttack.Value == true)
            {
                _weaponManager.PerformUltimateAttack();
                return;
            }

            bool normalCombat = gameManager.CombatState == CombatState.Normal;
           
            // Handle normal attack
            var normalAttackInput = SystemAPI.GetSingleton<PlayerNormalAttackInput>();
            if (normalAttackInput.KeyPressed && normalCombat)
            {
                _weaponManager.PerformNormalAttack();
                return;
            }

            // Handle special attack
            var specialAttack = SystemAPI.GetSingleton<PlayerSpecialAttackInput>();
            if (specialAttack.KeyDown && normalCombat)
            {
                _weaponManager.PerformSpecialAttack();
                return;
            }
        }
        
        protected override void OnStopRunning()
        {
            UnsubscribeFromAttackEvents();
        }
    }
}
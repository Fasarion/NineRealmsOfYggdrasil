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
using Random = UnityEngine.Random;

namespace Patrik
{
    public partial class PlayerAttackSystem : SystemBase
    {
        private PlayerWeaponManagerBehaviour _weaponManager;
        private bool hasSetUpWeaponManager;

        private AttackData previousActiveAttackData;
        private AttackData previousPassiveAttackData;

        CollisionFilter collisionFilter = new CollisionFilter()
        {
            BelongsTo = 1, // Projectile
            CollidesWith = 1 << 1 // Enemy
        };

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

                    _weaponManager.SetupWeapons();
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
            foreach (var ( collider, entity) in SystemAPI
                .Query<RefRW<PhysicsCollider>>()
                .WithAll<WeaponComponent>()
                .WithEntityAccess())
            {
                collider.ValueRW.Value.Value.SetCollisionFilter(CollisionFilter.Zero);
            }
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
            
            if (SystemAPI.HasComponent<ActiveWeapon>(entity))
            {
                SystemAPI.SetComponentEnabled<ActiveWeapon>(entity, true);
            }
            
            var data = new AttackData
            {
                AttackType = AttackType.Normal,
                WeaponType = type,
                ComboCounter = 0,
            };
            
            WriteOverAttackData(data);
        }
        
        private void SetWeaponPassive(WeaponType type)
        {
            Entity entity = GetWeaponEntity(type);
            
            if (SystemAPI.HasComponent<ActiveWeapon>(entity))
            {
                SystemAPI.SetComponentEnabled<ActiveWeapon>(entity, false);
            }
            
            var data = new AttackData
            {
                AttackType = AttackType.Passive,
                WeaponType = type,
                ComboCounter = 0,
            };
            
            WriteOverAttackData(data);
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
            weaponCaller.ValueRW.currentActiveAttackType = data.AttackType;
            weaponCaller.ValueRW.currentActiveWeaponType = data.WeaponType;
            weaponCaller.ValueRW.currentActiveCombo = data.ComboCounter;
            
            if (DifferentAttackData(data, previousActiveAttackData))
            {
                WriteOverAttackData(data);
                previousActiveAttackData = data;
            }
        }

        private bool DifferentAttackData(AttackData newData, AttackData lastData)
        {
            if (newData.AttackType != lastData.AttackType) return true;
            if (newData.ComboCounter != lastData.ComboCounter) return true;
          //  if (newData.WeaponType != lastData.WeaponType) return true;

            return false;
        }

        private void WriteOverAttackData(AttackData data)
        {
            bool statHandlerExists =
                SystemAPI.TryGetSingletonRW(out RefRW<StatHandlerComponent> statHandler);
            if (!statHandlerExists)
            {
                Debug.LogWarning("No stat handler exists, can't update stats.");
                return;
            }

            Debug.Log("Should update stats");
            statHandler.ValueRW.ShouldUpdateStats = true;
            statHandler.ValueRW.WeaponType = data.WeaponType;
            statHandler.ValueRW.AttackType = data.AttackType;
            statHandler.ValueRW.ComboCounter = data.ComboCounter;

            // TODO: Move this to seperate system at the moment when weapon is switched?
            Entity entity = GetWeaponEntity(data.WeaponType);
            var weapon = EntityManager.GetComponentData<WeaponComponent>(entity);
            weapon.InActiveState = data.AttackType != AttackType.Passive;

            weapon.CurrentAttackCombo = data.ComboCounter;
            weapon.CurrentAttackType = data.AttackType;
            
            EntityManager.SetComponentData(entity, weapon);
        }

        private void OnActiveAttackStop(AttackData data)
        {
            DisableWeapon(data.WeaponType);
        }

        private void OnPassiveAttackStart(AttackData data)
        {
            EnableWeapon(data.WeaponType);
            
            var weaponCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

            weaponCaller.ValueRW.shouldPassiveAttack = true;
            weaponCaller.ValueRW.currentPassiveWeaponType = data.WeaponType;
            
            if (DifferentAttackData(data, previousPassiveAttackData))
            {
                WriteOverAttackData(data);
                previousPassiveAttackData = data;
            }
            
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
            
            var collider = SystemAPI.GetComponentRW<PhysicsCollider>(entity);
            collider.ValueRW.Value.Value.SetCollisionFilter(collisionFilter);
        }

        private void DisableWeapon(WeaponType dataWeaponType)
        {
            Entity entity = GetWeaponEntity(dataWeaponType);
            
            var collider = SystemAPI.GetComponentRW<PhysicsCollider>(entity);
            collider.ValueRW.Value.Value.SetCollisionFilter(CollisionFilter.Zero);

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

public partial struct ResetAttackData : ISystem
{
    
}
using System;
using Damage;
using Health;
using Player;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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
                        // Missing Player Weapon Handler, attacks not possible.;
                        return;
                    }

                    DisableAllWeapons();
                    SubscribeToEvents();
                    hasSetUpWeaponManager = true;

                    _weaponManager.SetupWeapons();
                }
            }

            if (!_weaponManager) return;
            
            HandleWeaponStates();
            HandleWeaponSwitch();
            HandleWeaponInput();
        }

        private void HandleWeaponStates()
        {
            if (!SystemAPI.TryGetSingletonRW(out RefRW<WeaponAttackCaller> attackCaller))
                return;
            
            if (attackCaller.ValueRO.ResetWeaponCurrentWeaponTransform)
            {
                _weaponManager.ResetActiveWeapon();
                attackCaller.ValueRW.ResetWeaponCurrentWeaponTransform = false;
            }
        }

        private void HandleWeaponSwitch()
        {
            if (!SystemAPI.TryGetSingletonRW(out RefRW<WeaponAttackCaller> attackCaller))
                return;

            // don't switch weapon when preparing an attack
            if (attackCaller.ValueRO.IsPreparingAttack())
            {
                return;
            }

            // // don't switch mid attack
            // if (_weaponManager && _weaponManager.isAttacking)
            //     return; 

            if (attackCaller.ValueRO.ActiveAttackData.IsAttacking) return;

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

        private void SubscribeToEvents()
        {
            _weaponManager.OnActiveAttackStart += OnActiveAttackStart;
            _weaponManager.OnActiveAttackStop += OnActiveAttackStop;
            
            _weaponManager.OnPassiveAttackStart += OnPassiveAttackStart;
            _weaponManager.OnPassiveAttackStop += OnPassiveAttackStop;
            
            _weaponManager.OnSpecialCharge += OnSpecialCharge;
            _weaponManager.OnUltimatePrepare += OnUltimatePrepare;

            _weaponManager.OnWeaponActive += SetWeaponActive;
            _weaponManager.OnWeaponPassive += SetWeaponPassive;

            EventManager.OnBusyUpdate += OnBusyUpdate;
        }

        private void UnsubscribeFromAttackEvents()
        {
            _weaponManager.OnActiveAttackStart -= OnActiveAttackStart;
            _weaponManager.OnActiveAttackStop -= OnActiveAttackStop;
            
            _weaponManager.OnPassiveAttackStart -= OnPassiveAttackStart;
            _weaponManager.OnPassiveAttackStop -= OnPassiveAttackStop;
            
            _weaponManager.OnSpecialCharge -= OnSpecialCharge;
            _weaponManager.OnUltimatePrepare -= OnUltimatePrepare;

            _weaponManager.OnWeaponActive -= SetWeaponActive;
            _weaponManager.OnWeaponPassive -= SetWeaponPassive;
            
            EventManager.OnBusyUpdate -= OnBusyUpdate;
        }

        private void OnBusyUpdate(BusyAttackInfo info)
        {
            var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
            attackCaller.ValueRW.BusyAttackInfo = info;
        }
        
        private void OnUltimatePrepare(AttackData data)
        {
            TryWriteOverActiveAttackData(data);
        }

        private void OnSpecialCharge(AttackData data)
        {
            TryWriteOverActiveAttackData(data);
        }

        private void TryWriteOverActiveAttackData(AttackData data)
        {
            if (DifferentAttackData(data, previousActiveAttackData))
            {
                WriteOverAttackData(data);
                previousActiveAttackData = data;
            }
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

            var weaponCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
            weaponCaller.ValueRW.ActiveAttackData = new WeaponCallData()
            {
                ShouldStart = false,
                ShouldStop = false,
                AttackType = data.AttackType,
                WeaponType = data.WeaponType,
                Combo = data.ComboCounter
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

            weaponCaller.ValueRW.ActiveAttackData = new WeaponCallData()
            {
                ShouldStart = true,
                ShouldStop = false,
                IsAttacking = true,
                AttackType = data.AttackType,
                WeaponType = data.WeaponType,
                Combo = data.ComboCounter
            };
            
            TryWriteOverActiveAttackData(data);
        }

        private bool DifferentAttackData(AttackData newData, AttackData lastData)
        {
            if (newData.AttackType != lastData.AttackType) return true;
            if (newData.ComboCounter != lastData.ComboCounter) return true;

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
            
            var weaponCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

            weaponCaller.ValueRW.ActiveAttackData = new WeaponCallData()
            {
                ShouldStart = false,
                ShouldStop = true,
                IsAttacking = false,
                WeaponType = data.WeaponType,
                AttackType = data.AttackType,
                Combo = data.ComboCounter
            };
        }

        private void OnPassiveAttackStart(AttackData data)
        {
            EnableWeapon(data.WeaponType);
            
            var weaponCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

            weaponCaller.ValueRW.PassiveAttackData = new WeaponCallData()
            {
                ShouldStart = true,
                ShouldStop = false,
                IsAttacking = true,
                WeaponType = data.WeaponType,
                AttackType = data.AttackType,
                Combo = data.ComboCounter
            };

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
            
            var weaponCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

            weaponCaller.ValueRW.PassiveAttackData = new WeaponCallData()
            {
                ShouldStart = false,
                ShouldStop = true,
                IsAttacking = false,
                WeaponType = data.WeaponType,
                AttackType = AttackType.Passive
            };
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

            bool inCombatState = gameManager.GameState == GameState.Combat;
            if (!inCombatState)
                return;
            
            if (!SystemAPI.TryGetSingletonRW(out RefRW<WeaponAttackCaller> attackCaller))
                return;

            WeaponType currentWeapon = attackCaller.ValueRO.ActiveAttackData.WeaponType;

           // bool isPreparingAttack = attackCaller.ValueRO.IsPreparingAttack();
           // bool canAttack = !isPreparingAttack;
           
           bool canAttack = true;

           // reset ult flag
           _weaponManager.UpdateAttackAnimation(AttackType.Ultimate, false);
           
           // Handle ultimate perform
            if (attackCaller.ValueRO.PrepareUltimateInfo.Perform 
                && !attackCaller.ValueRO.BusyAttackInfo.IsBusy(AttackType.Ultimate, currentWeapon)
                && canAttack)
            {
                _weaponManager.UpdateAttackAnimation(AttackType.Ultimate, true);
                _weaponManager.PrepareUltimateAttack();
                canAttack = false;
            }
            // Handle ultimate prepare
            else if (attackCaller.ValueRO.PrepareUltimateInfo.HasPreparedThisFrame 
                     && !attackCaller.ValueRO.BusyAttackInfo.IsBusy(AttackType.Ultimate, currentWeapon)
                     && canAttack)// canAttack)
            {
                _weaponManager.PrepareUltimateAttack();
                canAttack = false;
            }

            if (canAttack == false)
            {
                Debug.Log("Ult activasio");
            }
            
            bool normalCombat = gameManager.CombatState == CombatState.Normal;
            if (!normalCombat)
                return;
            
            // Handle special charge
            var specialAttackInput = SystemAPI.GetSingleton<PlayerSpecialAttackInput>();
            bool specIsHeld = specialAttackInput.IsHeld;

            bool canSpecialAttack = canAttack && specIsHeld && !attackCaller.ValueRO.BusyAttackInfo.IsBusy(AttackType.Special, currentWeapon);//canAttack;
            _weaponManager.UpdateAttackAnimation(AttackType.Special, canSpecialAttack);

            if (canSpecialAttack)
            {
                canAttack = false;
            }
           
            // Handle normal attack
            var normalAttackInput = SystemAPI.GetSingleton<PlayerNormalAttackInput>();
            bool normIsHeld = normalAttackInput.IsHeld;

            bool canNormalAttack = normIsHeld && canAttack && !attackCaller.ValueRO.BusyAttackInfo.IsBusy(AttackType.Normal, currentWeapon);//canAttack;
            _weaponManager.UpdateAttackAnimation(AttackType.Normal, canNormalAttack);
            if (canNormalAttack)
            {
                canAttack = false;
            }

            // set charge info 
            attackCaller.ValueRW.SpecialChargeInfo = new SpecialChargeInfo
           {
               ChargingWeapon = _weaponManager.CurrentWeaponType, 
               chargeState = _weaponManager.chargeState
           };
            
           if (specialAttackInput.KeyUp)
           {
               _weaponManager.ReleaseSpecial();
           }
        }
        
        protected override void OnStopRunning()
        {
            UnsubscribeFromAttackEvents();
        }
    }
}
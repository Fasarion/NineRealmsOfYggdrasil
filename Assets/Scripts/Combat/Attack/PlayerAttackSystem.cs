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
            CollidesWith = 1 << 1 | 1 << 5, // Enemy
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

            bool gameIsPaused = !SystemAPI.HasSingleton<GameUnpaused>();
            if (gameIsPaused) return;

            if (!_weaponManager) return;
            
            HandleWeaponStates();
            HandleWeaponSwitch();
            HandleWeaponInput();
        }

        private void HandleWeaponStates()
        {
            if (!SystemAPI.TryGetSingletonRW(out RefRW<WeaponAttackCaller> attackCaller))
                return;
            
            if (attackCaller.ValueRO.ReturnWeapon)
            {
                _weaponManager.ReturnActiveWeapon();
                attackCaller.ValueRW.ReturnWeapon = false;
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

            // don't switch mid attack
            if (attackCaller.ValueRO.BusyAttackInfo.Busy)
                return; 

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

            if (SystemAPI.TryGetSingleton(out WeaponCycleRightInput cycleRightKey) && cycleRightKey.KeyPressed)
            {
                _weaponManager.CycleWeaponsRight();
            }
            
            if (SystemAPI.TryGetSingleton(out WeaponCycleLeftInput cycleLeftKey) && cycleLeftKey.KeyPressed)
            {
                _weaponManager.CycleWeaponsLeft();
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
            EventManager.OnActiveAttackStart += OnActiveAttackStart;
            EventManager.OnActiveAttackStop += OnActiveAttackStop;
            
            EventManager.OnPassiveAttackStart += OnPassiveAttackStart;
            EventManager.OnPassiveAttackStop += OnPassiveAttackStop;
            
            EventManager.OnSpecialCharge += OnSpecialCharge;
            EventManager.OnUltimatePrepare += OnUltimatePrepare;
            EventManager.OnUltimatePerform += OnUltimatePerform;
           
            EventManager.OnWeaponActive += SetWeaponActive;
            EventManager.OnWeaponPassive += SetWeaponPassive;

            EventManager.OnBusyUpdate += OnBusyUpdate;
            EventManager.OnChargeLevelChange += OnChargeLevelChange;
            
            EventManager.OnEnableMovementInput += OnEnableMovementInput;
            EventManager.OnEnableRotationInput += OnEnableRotationInput;
        }

        private void UnsubscribeFromAttackEvents()
        {
            EventManager.OnActiveAttackStart -= OnActiveAttackStart;
            EventManager.OnActiveAttackStop -= OnActiveAttackStop;

            EventManager.OnPassiveAttackStart -= OnPassiveAttackStart;
            EventManager.OnPassiveAttackStop -= OnPassiveAttackStop;

            EventManager.OnSpecialCharge -= OnSpecialCharge;
            EventManager.OnUltimatePrepare -= OnUltimatePrepare;
            EventManager.OnUltimatePerform -= OnUltimatePerform;

            EventManager.OnWeaponActive -= SetWeaponActive;
            EventManager.OnWeaponPassive -= SetWeaponPassive;

            EventManager.OnBusyUpdate -= OnBusyUpdate;
            EventManager.OnChargeLevelChange -= OnChargeLevelChange;
            
            EventManager.OnEnableMovementInput -= OnEnableMovementInput;
            EventManager.OnEnableRotationInput -= OnEnableRotationInput;
        }

        private void OnEnableRotationInput(bool enable)
        {
            // TODO: move to different system?
            
            if (!SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity playerEntity))
                return;

            EntityManager.SetComponentEnabled<CanRotateFromInput>(playerEntity, enable);
        }

        private void OnEnableMovementInput(bool enable)
        {
            // TODO: move to different system?
            
            if (!SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity playerEntity))
                return;

            EntityManager.SetComponentEnabled<CanMoveFromInput>(playerEntity, enable);
        }

        private void OnChargeLevelChange(int level)
        {
            var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
            attackCaller.ValueRW.SpecialChargeInfo.Level = level;
        }

        private void OnBusyUpdate(BusyAttackInfo info)
        {
            var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
            attackCaller.ValueRW.BusyAttackInfo = info;
        }
        
        private void OnUltimatePerform(WeaponType currentWeaponType, AttackData data)
        {
            TryWriteOverActiveAttackData(data);
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
            
            EntityManager.SetComponentEnabled<WeaponIsAttacking>(entity, true);
        }

        private void DisableWeapon(WeaponType dataWeaponType)
        {
            Entity entity = GetWeaponEntity(dataWeaponType);
            
            var collider = SystemAPI.GetComponentRW<PhysicsCollider>(entity);
            collider.ValueRW.Value.Value.SetCollisionFilter(CollisionFilter.Zero);
            
            EntityManager.SetComponentEnabled<WeaponIsAttacking>(entity, false);

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
                
                case WeaponType.Birds:
                    foreach (var (hammer, entity) in SystemAPI.Query<BirdsComponent>()
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
            ecb.Dispose();
        }

        private void HandleWeaponInput()
        {
            // // hammer upgrade test
            // if (Input.GetKeyDown(KeyCode.K))
            // {
            //     Debug.Log("Test upgrade");
            //
            //     var hammerEntity = SystemAPI.GetSingletonEntity<HammerSpecialConfig>();
            //     EntityManager.AddComponent<IsUnlocked>(hammerEntity);
            // }
            
            
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
            
            

           bool canAttack = true;


           // reset ult flag
           EventManager.OnUpdateAttackAnimation?.Invoke(AttackType.Ultimate, false);
           
           bool ultUnlocked = UltAttackUnlocked(currentWeapon);

           
           
           // Handle ultimate perform
            if (attackCaller.ValueRO.PrepareUltimateInfo.Perform 
                && !attackCaller.ValueRO.BusyAttackInfo.IsBusy(AttackType.Ultimate, currentWeapon)
                && canAttack && ultUnlocked)
            {
                EventManager.OnUpdateAttackAnimation?.Invoke(AttackType.Ultimate, true);
                //_weaponManager.PrepareUltimateAttack();
                _weaponManager.PerformUltimateAttack(currentWeapon);
                
                canAttack = false;
            }
            // Handle ultimate prepare
            else if (attackCaller.ValueRO.PrepareUltimateInfo.HasPreparedThisFrame 
                     && !attackCaller.ValueRO.BusyAttackInfo.IsBusy(AttackType.Ultimate, currentWeapon)
                     && canAttack && ultUnlocked)
            {
                _weaponManager.PrepareUltimateAttack();
                canAttack = false;
            }

            bool normalCombat = gameManager.CombatState == CombatState.Normal;
            if (!normalCombat)
                return;
            
            // Handle special charge
            var specialAttackInput = SystemAPI.GetSingleton<PlayerSpecialAttackInput>();
            bool specIsHeld = specialAttackInput.IsHeld;

            bool canSpecialAttack = canAttack && specIsHeld && !attackCaller.ValueRO.BusyAttackInfo.IsBusy(AttackType.Special, currentWeapon)
                && SpecialAttackUnlocked(currentWeapon);
            EventManager.OnUpdateAttackAnimation?.Invoke(AttackType.Special, canSpecialAttack);

            if (canSpecialAttack)
            {
                canAttack = false;
            }
           
            // Handle normal attack
            var normalAttackInput = SystemAPI.GetSingleton<PlayerNormalAttackInput>();
            bool normIsHeld = normalAttackInput.IsHeld;

            bool canNormalAttack = normIsHeld && canAttack && !attackCaller.ValueRO.BusyAttackInfo.IsBusy(AttackType.Normal, currentWeapon);
            EventManager.OnUpdateAttackAnimation?.Invoke(AttackType.Normal, canNormalAttack);
            if (canNormalAttack)
            {
                canAttack = false;
            }

            var oldChargeInfo = attackCaller.ValueRO.SpecialChargeInfo;

            // set charge info 
            attackCaller.ValueRW.SpecialChargeInfo = new SpecialChargeInfo
           {
               ChargingWeapon = _weaponManager.CurrentWeaponType, 
               chargeState = _weaponManager.chargeState,
               Level = oldChargeInfo.Level
           };

            if (_weaponManager.chargeState == ChargeState.Start)
            {
                // update stats when beginning charge
                var weaponEntity = GetWeaponEntity(_weaponManager.CurrentWeaponType);

                // TODO: hack to set attack type at start of charge
                var weapon = SystemAPI.GetComponent<WeaponComponent>(weaponEntity);
                weapon.CurrentAttackType = AttackType.Special;
                SystemAPI.SetComponent(weaponEntity, weapon);
                
                var statHandler = SystemAPI.GetSingletonRW<StatHandlerComponent>();
                statHandler.ValueRW.ShouldUpdateStats = true;
            }
            
           if (specialAttackInput.KeyUp)
           {
               _weaponManager.ReleaseSpecial();
           }
        }

        // private bool AttackUnlocked(WeaponType weaponType, AttackType attackType)
        // {
        //     switch (attackType)
        //     {
        //         case AttackType.Special:
        //             return SpecialAttackUnlocked(weaponType);
        //         
        //         case AttackType.Ultimate:
        //             return UltAttackUnlocked(weaponType);
        //     }
        //
        //     // assume every other attack is unlocked
        //     return true;
        // }
        
        private bool CheckForUnlock<T>(ComponentLookup<IsUnlocked> lookup) where T : unmanaged, IComponentData
        {
            bool entityExists = SystemAPI.TryGetSingletonEntity<T>(out Entity swordSpecial);
            bool lookupUnlock = entityExists && lookup.HasComponent(swordSpecial);

            return lookupUnlock;
        }
        
        private bool CheckForUnlock<T>() where T : unmanaged, IComponentData
        {
            bool entityExists = SystemAPI.TryGetSingletonEntity<T>(out Entity swordSpecial);
            bool lookupUnlock = entityExists && SystemAPI.HasComponent<IsUnlocked>(swordSpecial);

            return lookupUnlock;
        }

        
        private bool SpecialAttackUnlocked(WeaponType weaponType)
        {
            //var unlockLookup = SystemAPI.GetComponentLookup<IsUnlocked>();
            
            switch (weaponType)
            {
                case WeaponType.Sword:
                    return CheckForUnlock<IceRingConfig>();

                case WeaponType.Hammer:
                    return CheckForUnlock<HammerSpecialConfig>();
                
                case WeaponType.Birds:
                    return CheckForUnlock<BirdsSpecialAttackConfig>();

            }

            return false;
        }

        private bool UltAttackUnlocked(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.Sword:
                    return CheckForUnlock<SwordUltimateConfig>();
                
                case WeaponType.Hammer:
                    return CheckForUnlock<ThunderStrikeConfig>();

                case WeaponType.Birds:
                    return CheckForUnlock<BirdsUltimateAttackConfig>();
            }

            return false;
        }

        protected override void OnStopRunning()
        {
            UnsubscribeFromAttackEvents();
        }
    }
}
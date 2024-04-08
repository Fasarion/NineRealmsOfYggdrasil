using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Patrik
{
    public enum WeaponType
    {
        None = 0,
        Sword = 1,
        Hammer = 2,
        Mead = 3,
        Birds = 4
    }

    public enum AttackType
    {
        None = 0,
        Normal = 1,
        Special = 2,
        Ultimate = 3,
        Passive = 4
    }
    
    public class PlayerWeaponManagerBehaviour : MonoBehaviour
    {
        public static PlayerWeaponManagerBehaviour Instance { get; private set; }

        [Header("Weapon")]
        [SerializeField] private WeaponType startWeaponType = WeaponType.Sword;

        [Header("Animation")]
        [SerializeField] private Animator playerAnimator;
        
        [Header("Weapon Slots")]
        [SerializeField] private Transform activeSlot;
        [SerializeField] private List<Transform> passiveSlots = new List<Transform>();

        // Weapons
        private List<WeaponBehaviour> weapons;
        private WeaponBehaviour activeWeapon;
        
        // Attack Data
        private AttackType CurrentAttackType { get;  set; }
        public bool isAttacking;
        
        // Animator parameters
        private string attackAnimationName = "Attack";
        private string activeWeaponParameterName = "ActiveWeapon";
        private string currentAttackParameterName = "CurrentAttack";

        private string movingParameterName = "Moving";
        
        // Events
        public UnityAction<AttackData> OnActiveAttackStart;
        public UnityAction<AttackData> OnActiveAttackStop;
        
        // Events
        public UnityAction<AttackData> OnPassiveAttackStart;
        public UnityAction<AttackData> OnPassiveAttackStop;

        
        // Events called from animator. NOTE: DO NOT REMOVE BECAUSE THEY ARE GREYED OUT IN EDITOR
        public void StartActiveAttackEvent()
        {
            OnActiveAttackStart?.Invoke(GetActiveAttackData());
        } 
        
        public void StopActiveAttackEvent()
        {
            OnActiveAttackStop?.Invoke(GetActiveAttackData()); 
        }

        public void FinishActiveAttackAnimationEvent()
        {
            isAttacking = false;
        }

        /// <summary>
        /// Attack Data from current attack. Informs DOTS which weapon was attacking, which attack type was used and
        /// at which point the attack occured.
        /// </summary>
        /// <returns></returns>
        private AttackData GetActiveAttackData()
        {
            var attackData = new AttackData
            {
                AttackType = CurrentAttackType,
                WeaponType = activeWeapon.WeaponType,
                AttackPoint = activeWeapon.AttackPoint
            };

            return attackData;
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            // wait a few frames to setup weapons to make sure they have spawned from the DOTS side
            Invoke(nameof(SetupWeapons), 0.1f);
        }

        private void OnDisable()
        {
            foreach (var weapon in weapons)
            {
                UnsubscribeFromPassiveEvents(weapon);
            }
        }

        private void SetupWeapons()
        {
            weapons = FindObjectsOfType<WeaponBehaviour>().ToList();

            int passiveSlotCounter = 0;
            
            foreach (var weapon in weapons)
            {
                // Handle active weapon
                if (weapon.WeaponType == startWeaponType)
                {
                    activeWeapon = weapon;
                    weapon.MakeActive(activeSlot, true);
                    continue;
                }
                
                // Handle passive weapon
                Transform passiveParent = passiveSlotCounter <= passiveSlots.Count
                    ? passiveSlots[passiveSlotCounter]
                    : activeSlot;
                weapon.MakeActive(passiveParent, false);
                passiveSlotCounter++;

                SubscribeToPassiveEvents(weapon);
            }
        }

        private void SubscribeToPassiveEvents(WeaponBehaviour weapon)
        {
            weapon.OnPassiveAttackStart += StartPassiveAttack;
            weapon.OnPassiveAttackStop += StopPassiveAttack;
        }
        
        private void UnsubscribeFromPassiveEvents(WeaponBehaviour weapon)
        {
            weapon.OnPassiveAttackStart -= StartPassiveAttack;
            weapon.OnPassiveAttackStop -= StopPassiveAttack;
        }

        private void StartPassiveAttack(WeaponBehaviour weapon)
        {
            OnPassiveAttackStart?.Invoke(GetPassiveAttackData(weapon));
        }
        
        private void StopPassiveAttack(WeaponBehaviour weapon)
        {
            OnPassiveAttackStop?.Invoke(GetPassiveAttackData(weapon));
        }

        private static AttackData GetPassiveAttackData(WeaponBehaviour weapon)
        {
            var attackData = new AttackData
            {
                AttackType = AttackType.Passive,
                WeaponType = weapon.WeaponType,
                AttackPoint = weapon.AttackPoint
            };

            return attackData;
        }

        /// <summary>
        /// Function to be called when a normal attack is about to be performed. Called from DOTS after the correct input
        /// is registered.
        /// </summary>
        public void PerformNormalAttack()
        {
            TryPerformAttack(AttackType.Normal);
        }
        
        /// <summary>
        /// Function to be called when a special attack is about to be performed. Called from DOTS after the correct input
        /// is registered.
        /// </summary>
        public void PerformSpecialAttack()
        {
            TryPerformAttack(AttackType.Special);
        }
        
        /// <summary>
        /// Function to be called when a special attack is about to be performed. Called from DOTS after the correct input
        /// is registered.
        /// </summary>
        public void PerformUltimateAttack()
        {
            //TODO: Implement ultimate attack
        }

        private void TryPerformAttack(AttackType type)
        {
            //TODO: fix isAttacking parameter
            // if (isAttacking) return;

            isAttacking = true;
            CurrentAttackType = type;
            UpdateAnimatorAttackParameters();
        }

        private void UpdateAnimatorAttackParameters()
        {
            playerAnimator.SetInteger(currentAttackParameterName, (int) CurrentAttackType);
            playerAnimator.SetInteger(activeWeaponParameterName, (int) activeWeapon.WeaponType);
            
            playerAnimator.Play(attackAnimationName);
        }

        public void UpdateMovementParameter(bool playerIsMoving)
        {
            playerAnimator.SetBool(movingParameterName, playerIsMoving);
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Patrik
{
    public enum WeaponType
    {
        None = 0,
        Sword = 1,
        Axe = 2,
        Mead = 3,
        Birds = 4
    }

    public enum AttackType
    {
        None = 0,
        Normal = 1,
        Special = 2,
        Ultimate = 3
    }
    
    public class PlayerWeaponManagerBehaviour : MonoBehaviour
    {
        public static PlayerWeaponManagerBehaviour Instance { get; private set; }

        [SerializeField] private Animator playerAnimator;
        [SerializeField] private List<WeaponBehaviour> weapons;

        // Weapons
        private static int PASSIVE_WEAPON_COUNT = 2;

        private WeaponBehaviour[] passiveWeapons = new WeaponBehaviour[PASSIVE_WEAPON_COUNT];
        private WeaponBehaviour activeWeapon;
        private AttackType CurrentAttackType { get;  set; }
        
        // Animator parameter names
        private string attackAnimationName = "Attack";
        private string activeWeaponParameterName = "ActiveWeapon";
        private string currentAttackAnimationName = "CurrentAttack";
        
        // Events
        public UnityAction<AttackData> OnAttackStart;
        public UnityAction<AttackData> OnAttackStop;

        
        // Events called from animator. NOTE: DO NOT REMOVE BECAUSE THEY ARE GREYED OUT IN EDITOR
        public void OnStartAttackEvent() => OnAttackStart?.Invoke(GetAttackData());
        
        public void OnStopAttackEvent() => OnAttackStop?.Invoke(GetAttackData());
        
        /// <summary>
        /// Attack Data from current attack. Informs DOTS which weapon was attack, which attack type was used and
        /// at which point the attack occured.
        /// </summary>
        /// <returns></returns>
        private AttackData GetAttackData()
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
            activeWeapon = weapons[0];
        }

        /// <summary>
        /// Function to be called when a normal attack is about to be performed. Called from DOTS after the correct input
        /// is registered.
        /// </summary>
        public void PerformNormalAttack()
        {
            PerformAttack(AttackType.Normal);
        }
        
        /// <summary>
        /// Function to be called when a special attack is about to be performed. Called from DOTS after the correct input
        /// is registered.
        /// </summary>
        public void PerformSpecialAttack()
        {
            PerformAttack(AttackType.Special);
        }
        
        /// <summary>
        /// Function to be called when a special attack is about to be performed. Called from DOTS after the correct input
        /// is registered.
        /// </summary>
        public void PerformUltimateAttack()
        {
            //TODO: Implement ultimate attack
        }

        private void PerformAttack(AttackType type)
        {
            CurrentAttackType = type;
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            playerAnimator.SetInteger(currentAttackAnimationName, (int) CurrentAttackType);
            playerAnimator.SetInteger(activeWeaponParameterName, (int) activeWeapon.WeaponType);

            playerAnimator.Play(attackAnimationName);
        }
    }
}
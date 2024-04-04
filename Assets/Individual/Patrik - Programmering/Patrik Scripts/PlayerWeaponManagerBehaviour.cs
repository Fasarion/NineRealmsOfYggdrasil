using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Patrik
{
    public class PlayerWeaponManagerBehaviour : MonoBehaviour
    {
        public static PlayerWeaponManagerBehaviour Instance { get; private set; }

        [SerializeField] private List<WeaponBehaviour> weapons;


        private static int PASSIVE_WEAPON_COUNT = 2;

        private WeaponBehaviour[] passiveWeapons = new WeaponBehaviour[PASSIVE_WEAPON_COUNT];
        private WeaponBehaviour activeWeapon;

        public UnityAction OnActiveWeaponStartAttackNormal;
        public UnityAction OnActiveWeaponStopAttackNormal;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            foreach (var weapon in weapons)
            {
                weapon.OnAttackPerformed += OnAttackPerformed;
                weapon.OnAttackStop += OnAttackStop;
            }

            activeWeapon = weapons[0];
        }
        
        private void OnDisable()
        {
            foreach (var weapon in weapons)
            {
                weapon.OnAttackPerformed -= OnAttackPerformed;
                weapon.OnAttackStop -= OnAttackStop;
            }
        }

        private void OnAttackPerformed()
        {
            OnActiveWeaponStartAttackNormal?.Invoke();
        }
        
        private void OnAttackStop()
        {
            OnActiveWeaponStopAttackNormal?.Invoke();
        }

        public void NormalAttack()
        {
            activeWeapon.PerformNormalAttack();
        }
        
        public void SpecialAttack()
        {
            activeWeapon.PerformSpecialAttack();
        }
    }
}
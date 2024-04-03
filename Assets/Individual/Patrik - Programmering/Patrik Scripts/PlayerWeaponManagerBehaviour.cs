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

        public UnityAction OnActiveWeaponAttack;
        public UnityAction OnActiveWeaponStopAttack;

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
            OnActiveWeaponAttack?.Invoke();
        }
        
        private void OnAttackStop()
        {
            OnActiveWeaponStopAttack?.Invoke();
        }

        public void TryPerformCurrentAttack()
        {
            activeWeapon.PerformNormalAttack();
        }
    }
}
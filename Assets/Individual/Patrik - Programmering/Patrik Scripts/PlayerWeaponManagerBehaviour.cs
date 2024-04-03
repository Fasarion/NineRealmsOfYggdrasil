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

        // temp way of accessing current weapon
        private WeaponBehaviour currentWeapon => weapons[0];

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
            currentWeapon.PerformAttack();
        }
    }
}
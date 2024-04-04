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

        public UnityAction<AttackData> OnActiveWeaponStartAttackNormal;
        public UnityAction<AttackData> OnActiveWeaponStopAttackNormal;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            foreach (var weapon in weapons)
            {
                weapon.OnAttackPerformed += OnNormalAttackStart;
                weapon.OnAttackStop += OnNormalAttackStop;
            }

            activeWeapon = weapons[0];
        }
        
        private void OnDisable()
        {
            foreach (var weapon in weapons)
            {
                weapon.OnAttackPerformed -= OnNormalAttackStart;
                weapon.OnAttackStop -= OnNormalAttackStop;
            }
        }

        private void OnNormalAttackStart(AttackData data)
        {
            OnActiveWeaponStartAttackNormal?.Invoke(data);
        }
        
        private void OnNormalAttackStop(AttackData data)
        {
            OnActiveWeaponStopAttackNormal?.Invoke(data);
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
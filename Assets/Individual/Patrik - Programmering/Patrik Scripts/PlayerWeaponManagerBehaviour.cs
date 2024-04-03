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
        [SerializeField] private SwordBehaviour swordBehaviour;

        public UnityAction OnPerformAttack;
        public UnityAction OnStopAttack;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            swordBehaviour.OnAttackPerformed += OnAttackPerformed;
            swordBehaviour.OnAttackStop += OnAttackStop;
        }
        
        private void OnDisable()
        {
            swordBehaviour.OnAttackPerformed -= OnAttackPerformed;
            swordBehaviour.OnAttackStop += OnAttackStop;
        }

        private void OnAttackPerformed()
        {
            OnPerformAttack?.Invoke();
        }
        
        private void OnAttackStop()
        {
            OnStopAttack?.Invoke();
        }

        public void TryPerformCurrentAttack()
        {
            swordBehaviour.PerformAttack();
        }
    }
}
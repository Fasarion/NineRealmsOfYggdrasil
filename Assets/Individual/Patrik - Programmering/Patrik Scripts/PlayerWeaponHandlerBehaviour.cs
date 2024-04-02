using System;
using UnityEngine;
using UnityEngine.Events;

namespace Patrik
{
    public class PlayerWeaponHandlerBehaviour : MonoBehaviour
    {
        public static PlayerWeaponHandlerBehaviour Instance { get; private set; }

        [SerializeField] private SwordBehaviour swordBehaviour;

        public UnityAction OnPerformAttack;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            swordBehaviour.OnAttackPerformed += OnAttackPerformed;
        }
        
        private void OnDisable()
        {
            swordBehaviour.OnAttackPerformed -= OnAttackPerformed;
        }

        private void OnAttackPerformed()
        {
            OnPerformAttack?.Invoke();
        }

        public void TryPerformCurrentAttack()
        {
            swordBehaviour.PerformAttack();
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Patrik
{
    public class PlayerWeaponHandlerBehaviour : MonoBehaviour
    {
        public static PlayerWeaponHandlerBehaviour Instance { get; private set; }

        [SerializeField] private SwordBehaviour swordBehaviour;
        public Transform SwordTip => swordBehaviour.Tip;

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
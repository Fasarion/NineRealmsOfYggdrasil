using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Patrik
{
    public class SwordBehaviour : WeaponBehaviour
    {
        [SerializeField] private Transform tip;

        public static SwordBehaviour Instance;
        
        public Transform Tip => tip;
        
        private void Awake()
        {
            Instance = this;
        }

    }

    public abstract class WeaponBehaviour : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private string attackAnimationName = "Attack";

        public UnityAction OnAttackPerformed;
        public UnityAction OnAttackStop;

        public void OnStartAttackEvent()
        {
            OnAttackPerformed?.Invoke();
        }

        public void OnStopAttackEvent()
        {
            OnAttackStop?.Invoke();
        }

        public void PerformAttack()
        {
            animator.Play(attackAnimationName);
        }
    }
}


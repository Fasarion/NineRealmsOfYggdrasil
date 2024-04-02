using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Patrik
{
    public class SwordBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform tip;
        [SerializeField] private Animator swordAnimator;
        
        private string swordAttackName = "Swing";

        public UnityAction OnAttackPerformed;
    
        public void OnAttackEvent()
        {
            OnAttackPerformed?.Invoke();
        }

        public void PerformAttack()
        {
            swordAnimator.SetTrigger(swordAttackName);
        }
    }
}


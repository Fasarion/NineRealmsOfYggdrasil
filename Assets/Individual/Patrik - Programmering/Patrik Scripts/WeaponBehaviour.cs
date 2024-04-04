using UnityEngine;
using UnityEngine.Events;

namespace Patrik
{
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private string normalAttackAnimationName = "NormalAttack";
        private string specialAttackAnimationName = "SpecialAttack";

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

        public void PerformNormalAttack()
        {
            animator.Play(normalAttackAnimationName);
        }
        
        public void PerformSpecialAttack()
        {
            animator.Play(specialAttackAnimationName);
        }
    }
}
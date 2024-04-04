using UnityEngine;
using UnityEngine.Events;

namespace Patrik
{
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform attackPoint;
        public Transform AttackPoint => attackPoint;

        private string normalAttackAnimationName = "NormalAttack";
        private string specialAttackAnimationName = "SpecialAttack";

        public UnityAction<AttackData> OnAttackPerformed;
        public UnityAction<AttackData> OnAttackStop;

        public void OnStartAttackEvent()
        {
            OnAttackPerformed?.Invoke(GetAttackData());
        }

        private AttackData GetAttackData()
        {
            var attackData = new AttackData();
            attackData.Position = attackPoint.position;

            return attackData;
        }

        public void OnStopAttackEvent()
        {
            OnAttackStop?.Invoke(GetAttackData());
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
using UnityEngine;
using UnityEngine.Events;

namespace Patrik
{
    public enum WeaponType
    {
        None,
        Sword,
        Axe,
        Mead,
        Birds
    }
    
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        [SerializeField] private WeaponType _weaponType;
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
            var attackData = new AttackData
            {
                WeaponType = _weaponType,
                AttackPoint = attackPoint
            };

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
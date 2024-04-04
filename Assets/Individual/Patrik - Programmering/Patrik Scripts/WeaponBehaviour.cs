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

    public enum AttackType
    {
        Normal, 
        Special,
        Ultimate
    }
    
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        [SerializeField] private WeaponType _weaponType;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform attackPoint;
        public Transform AttackPoint => attackPoint;
        public AttackType CurrentAttackType { get; private set; }

        private string normalAttackAnimationName = "NormalAttack";
        private string specialAttackAnimationName = "SpecialAttack";

        public UnityAction<AttackData> OnAttackPerformed;
        public UnityAction<AttackData> OnAttackStop;

        private AttackData GetAttackData()
        {
            var attackData = new AttackData
            {
                AttackType = CurrentAttackType,
                WeaponType = _weaponType,
                AttackPoint = attackPoint
            };

            return attackData;
        }
        
        // Animator Events
        public void OnStartAttackEvent() => OnAttackPerformed?.Invoke(GetAttackData());
        
        public void OnStopAttackEvent() => OnAttackStop?.Invoke(GetAttackData());
        

        public void PerformNormalAttack()
        {
            CurrentAttackType = AttackType.Normal;
            animator.Play(normalAttackAnimationName);
        }
        
        public void PerformSpecialAttack()
        {
            CurrentAttackType = AttackType.Special;
            animator.Play(specialAttackAnimationName);
        }
    }
}
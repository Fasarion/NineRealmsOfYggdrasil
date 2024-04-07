using UnityEngine;
using UnityEngine.Events;

namespace Patrik
{
    public class WeaponBehaviour : MonoBehaviour
    {
        [SerializeField] private WeaponType _weaponType;
        public WeaponType WeaponType => _weaponType;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform attackPoint;
        public Transform AttackPoint => attackPoint;
        private bool activeState;
        
        // Events
        public UnityAction<WeaponBehaviour> OnPassiveAttackStart;
        public UnityAction<WeaponBehaviour> OnPassiveAttackStop;

        
        // Events called from animator. NOTE: DO NOT REMOVE BECAUSE THEY ARE GREYED OUT IN EDITOR
        public void StartActiveAttackEvent() => OnPassiveAttackStart?.Invoke(this);
        public void StopActiveAttackEvent() => OnPassiveAttackStart?.Invoke(this);
        
        public void MakeActive(Transform parent, bool activeWeapon)
        {
            activeState = activeWeapon;
            bool enableAnimator = !activeState;
            
            animator.enabled = enableAnimator;
            
            transform.parent = parent;
            transform.position = parent.position;
            transform.rotation = parent.rotation;
        }
    }
}
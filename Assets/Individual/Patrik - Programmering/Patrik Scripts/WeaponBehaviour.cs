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
        [SerializeField] private Transform model;
        public Transform AttackPoint => attackPoint;
        private bool activeState;
        
        // Events
        public UnityAction<WeaponBehaviour> OnPassiveAttackStart;
        public UnityAction<WeaponBehaviour> OnPassiveAttackStop;

        
        // Events called from animator. NOTE: DO NOT REMOVE BECAUSE THEY ARE GREYED OUT IN EDITOR
        public void StartActiveAttackEvent() => OnPassiveAttackStart?.Invoke(this);
        public void StopActiveAttackEvent() => OnPassiveAttackStop?.Invoke(this);
        
        public void MakeActive(Transform parent)
        {
            activeState = true;
            animator.enabled = false;
            
            SetParent(parent);
        }

        private void SetParent(Transform parent)
        {
            transform.parent = parent;
            transform.position = parent.position;
            transform.rotation = parent.rotation;
        }

        public void MakePassive(Transform parent)
        {
            activeState = false;
            animator.enabled = true;
            
            SetParent(parent);

            ResetAnimationModel();
            StopActiveAttackEvent();
        }

        private void ResetAnimationModel()
        {
            model.position = transform.position;
            model.rotation = transform.rotation;        
        }
    }
}
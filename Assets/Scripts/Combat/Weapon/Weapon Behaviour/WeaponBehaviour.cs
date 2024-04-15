using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Patrik
{
    public class WeaponBehaviour : MonoBehaviour
    {
        [Header("Weapon Type")]
        [SerializeField] private WeaponType _weaponType;
        public WeaponType WeaponType => _weaponType;
        
        [Header("Animation")]
        [SerializeField] private Animator animator;

        [Header("Graphics")] 
        [SerializeField] private Sprite sprite;
        public Sprite Sprite => sprite;
        
        [Header("Transforms")]
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
            var transform1 = transform;
            transform1.parent = parent;
            transform1.position = parent.position;
            transform1.rotation = parent.rotation;
            
            model.position = transform1.position;
            model.rotation = transform1.rotation;
            model.localScale = transform1.localScale;
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
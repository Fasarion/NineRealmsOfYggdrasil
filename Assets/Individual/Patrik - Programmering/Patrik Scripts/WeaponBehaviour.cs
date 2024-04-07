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
        
        public void MakeActive(Transform parent, bool activeWeapon)
        {
            bool enableAnimator = !activeWeapon;
            
            animator.enabled = enableAnimator;
            
            transform.parent = parent;
            transform.position = parent.position;
            transform.rotation = parent.rotation;
        }

        public void MakeInActive(PlayerWeaponManagerBehaviour weaponManagerBehaviour)
        {
            animator.enabled = true;
        }
    }
}
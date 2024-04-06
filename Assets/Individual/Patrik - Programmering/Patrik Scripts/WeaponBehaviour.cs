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
        
        public void MakeActive(PlayerWeaponManagerBehaviour weaponManagerBehaviour)
        {
            animator.enabled = false;
            
            transform.parent = weaponManagerBehaviour.ActiveSlot;
            transform.position = weaponManagerBehaviour.ActiveSlot.position;
            transform.rotation = weaponManagerBehaviour.ActiveSlot.rotation;
        }

        public void MakeInActive(PlayerWeaponManagerBehaviour weaponManagerBehaviour)
        {
            animator.enabled = true;
        }
    }
}
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
        
        public static WeaponBehaviour Instance;

        public void MakeActive(PlayerWeaponManagerBehaviour weaponManagerBehaviour)
        {
            transform.parent = weaponManagerBehaviour.WeaponSlot;
            transform.position = weaponManagerBehaviour.WeaponSlot.position;
            transform.rotation = weaponManagerBehaviour.WeaponSlot.rotation;
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}
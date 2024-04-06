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

        private void Awake()
        {
            Instance = this;
        }
    }
}
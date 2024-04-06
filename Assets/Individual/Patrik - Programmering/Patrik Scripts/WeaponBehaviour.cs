using UnityEngine;
using UnityEngine.Events;

namespace Patrik
{
  
    
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        [SerializeField] private WeaponType _weaponType;
        public WeaponType WeaponType => _weaponType;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform attackPoint;
        public Transform AttackPoint => attackPoint;
    }
}
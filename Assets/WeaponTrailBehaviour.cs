using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class WeaponTrailBehaviour : MonoBehaviour
{
    [SerializeField] private WeaponType weaponType;
    
    // private void OnEnable()
    // {
    //     
    //     
    //     PlayerWeaponManagerBehaviour.Instance.OnActiveAttackStart += OnActiveAttackStart;
    // }
    //
    // private void OnDisable()
    // {
    //     PlayerWeaponManagerBehaviour.Instance.OnActiveAttackStart -= OnActiveAttackStart;
    // }

    private void OnActiveAttackStart(AttackData attackData)
    {
        if (attackData.WeaponType != weaponType) return;
        
        Debug.Log("Listen to attack call");
    }
}

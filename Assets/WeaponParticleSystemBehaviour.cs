using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class WeaponParticleSystemBehaviour : MonoBehaviour
{
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private ParticleSystem _particleSystem;
    
    [Tooltip("Which attack types will trigger this particle system to activate?")]
    [SerializeField] private List<AttackType> attacksToActivateParticles;

    private void OnEnable()
    {
        EventManager.OnActiveAttackStart += OnActiveAttackStart;
        EventManager.OnActiveAttackStop += OnActiveAttackStop;
        
        EventManager.OnPassiveAttackStart += OnPassiveAttackStart;
        EventManager.OnPassiveAttackStop += OnPassiveAttackStop;

        EventManager.OnWeaponSwitch += OnWeaponSwitch;
    }
    
    private void OnDisable()
    {
        EventManager.OnActiveAttackStart -= OnActiveAttackStart;
        EventManager.OnActiveAttackStop -= OnActiveAttackStop;

        EventManager.OnPassiveAttackStart -= OnPassiveAttackStart;
        EventManager.OnPassiveAttackStop -= OnPassiveAttackStop;
        
        EventManager.OnWeaponSwitch -= OnWeaponSwitch;
    }

    private void OnWeaponSwitch(WeaponSetupData weaponData, List<WeaponSetupData> allWeapons)
    {
        if (weaponData.WeaponType == weaponType)
        {
            SetParticleSystemActive(false);
        }
    }

    void HandleParticleActivision(AttackData attackData, bool activate)
    {
        if (attackData.WeaponType != weaponType) return;

        if (attacksToActivateParticles.Contains(attackData.AttackType))
        {
            SetParticleSystemActive(activate);
        }
    }

    private void SetParticleSystemActive(bool activate)
    {
        _particleSystem.gameObject.SetActive(activate);
    }

    private void OnActiveAttackStart(AttackData attackData)
    {
        HandleParticleActivision(attackData, true);
    }
    
    private void OnActiveAttackStop(AttackData attackData)
    {
        HandleParticleActivision(attackData, false);
    }
    
    private void OnPassiveAttackStart(AttackData attackData)
    {
        HandleParticleActivision(attackData, true);
    }
    
    private void OnPassiveAttackStop(AttackData attackData)
    {
        HandleParticleActivision(attackData, false);
    }
}

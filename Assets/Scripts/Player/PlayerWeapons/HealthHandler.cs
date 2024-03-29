using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthHandler", menuName = "EventHandlers/HealthHandler" +
    "")]
public class HealthHandler : ScriptableObject
{
    //Variables
    [SerializeField] private float startingHealth = 100;
    [SerializeField] private float maxHealth = 100;
    [SerializeField]private float currentHealth;
    
    //Events
    public event Action<float> onHealthChanged;
    public event Action onDamageTaken; 
    public event Action onPlayerDeath;

    public PlayerAudio playerAudio;

    //Reference variables


    //this variable is updated (set) by other scripts that want the player to take damage
    public float CurrentHealth 
    {
        get
        {
            return currentHealth;
        }
        set
        {
            float newHealth = currentHealth + value;
            
            if(value < 0) DamageTaken();

            currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
            
            HealthChanged(currentHealth);
            
            if (currentHealth <= 0)
            {
                PlayerDeath();
            }
        }
    }

    public float MaxHealth 
    {
        get => maxHealth;
        set => maxHealth = value;
    }
    
    //Functions
    //only called from this object
    private void HealthChanged(float amount)
    {
        onHealthChanged?.Invoke(amount);
    }

    private void DamageTaken()
    {
        onDamageTaken?.Invoke();
    }

    private void PlayerDeath()
    {
        onPlayerDeath?.Invoke();
    }

    public void Reset()
    {
        currentHealth = startingHealth;
    }

    private void OnEnable()
    {
        CurrentHealth = startingHealth;
    }



}

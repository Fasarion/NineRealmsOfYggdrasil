using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public delegate void UseActiveAttack();

public class PlayerWeaponBehaviour : MonoBehaviour
{
    [SerializeField] private SwordActiveWeapon sword;
    //[SerializeField] private SpearPassiveWeapon spear;
    //[SerializeField] private HammerPassiveWeapon hammer;
    //[SerializeField] private BowPassiveWeapon bow;
    //[SerializeField] private ShurikenPassiveWeapon shuriken;
    
    public List<GameObject> playerActiveWeapons = new List<GameObject>();

    public PlayerAudio playerAudio; 
    public event UseActiveAttack activeAttackUsed;
    public event Action activeAttackStopped;

    public event Action onPowerUsed;

    public float timer = 0f;
    public float powerCooldown = 5f;
    public bool isPowerOnCooldown;

    private void Awake()
    {
        sword = FindObjectOfType<SwordActiveWeapon>();
        playerActiveWeapons.Add(sword.GameObject());

    }

    public void PowerUsed()
    {
        onPowerUsed?.Invoke();
    }
    
    public void ActiveAttack()
    {
        activeAttackUsed?.Invoke();
    }

    public void StopAttack()
    {
        activeAttackStopped?.Invoke();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isPowerOnCooldown)
        {
            PowerUsed();
            timer = 0;
            isPowerOnCooldown = true;
        }

        if (timer >= powerCooldown)
        {
            isPowerOnCooldown = false;
        }
        else timer += Time.deltaTime;
    }
}

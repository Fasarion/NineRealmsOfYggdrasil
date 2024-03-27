using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    
    private MeshRenderer meshRenderer;
    [HideInInspector]public InputHandler inputHandler;
    [HideInInspector]public PlayerMovement playerMovement;

    public GameObject vfxHolder;

    public GameObject healthPrefab;
    public GameObject goldPrefab;
    
    public List<GameObject> healthVfx = new List<GameObject>();
    public List<GameObject> goldVfx = new List<GameObject>();
    
    void Awake()
    {
        GetComponent<PlayerMovement>();
    }

    public void OnEnable()
    {
    }

    public void OnDisable()
    {
       
    }

    private void OnPlayerDeath()
    {
        //renderer.material.color = new Color(1, 1, 1, 0);
        Destroy(transform.parent.gameObject);
    }
}

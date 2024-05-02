using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIHealthBar : MonoBehaviour
{

    public Image imageRenderer;
    public TMP_Text healthText;
    
    public List<Sprite> sprites;

    private int currentHealth;
    private int maxHealth;
    // Start is called before the first frame update

    private List<float> levels;

    //private Sprite currentSprite;
    private bool maxHealthSet = false;
  

    public void Awake()
    {
      LevelsSet();
    }
    void Start()
    {
    }

    public void OnEnable()
    {
        EventManager.OnPlayerHealthSet += OnHealthSet;
    }
    public void OnDisable()
    {
        EventManager.OnPlayerHealthSet -= OnHealthSet;
    }
    

    // Update is called once per frame
    void Update()
    {
        //SetCurrentSprite();
    }

    public void SetCurrentSprite()
    {
        //Makes it so the health is always in a span of 0 to 1.
        if (maxHealth != 0)
        {
            var currentHealthNormalized = currentHealth / (float)maxHealth;
          

            for (int i = levels.Count-1; i >= 0; i--)
            {
                if (currentHealthNormalized <= levels[i])
                {
                    imageRenderer.sprite = sprites[i];
                }
            }
        }
        
    }

    //Callback for when the current health of the player is set.
    private void OnHealthSet(PlayerHealthData healthData)
    {
        currentHealth = (int)healthData.currentHealth;
        maxHealth = (int) healthData.maxHealth;
        SetCurrentSprite();
    }
    
    //Callback for when the maxHealth of the player has been set at game start. 
    private void LevelsSet()
    {
        levels = new List<float>();
        for (int i = 0; i < sprites.Count; i++)
        {
            levels.Add((float)i / (sprites.Count-1));
        }
    }
}

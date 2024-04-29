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

  

    public void Awake()
    {
        
    }
    void Start()
    {
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
    public void OnHealthUpdated(int currentNewHealth)
    {
        currentHealth = currentNewHealth;
    }
    
    //Callback for when the maxHealth of the player has been set at game start. 
    public void OnMaxHealthSet()
    {
        levels = new List<float>();
        for (int i = 0; i < sprites.Count; i++)
        {
            levels.Add((float)i / (sprites.Count-1));
        }

        SetCurrentSprite();
    }
}

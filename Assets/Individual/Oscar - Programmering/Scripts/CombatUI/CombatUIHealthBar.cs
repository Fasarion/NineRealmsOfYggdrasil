using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUIHealthBar : MonoBehaviour
{

    public List<Sprite> sprites;

    private int currentHealth;
    private int maxHealth;
    // Start is called before the first frame update

    private List<float> levels;

    private Sprite currentSprite;
    void Start()
    {
        SetCurrentSprite();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCurrentSprite()
    {
        //Makes it so the health is always in a span of 0 to 1.
        var currentHealthNormalized = currentHealth / maxHealth;
        for (int i = 0; i < sprites.Count; i++)
        {
            levels.Add((float)i / sprites.Count);
        }

        for (int i = levels.Count-1; i > 0; i--)
        {
            if (currentHealthNormalized <= levels[i])
            {
                //currentSprite = 
            }
        }
    }
}

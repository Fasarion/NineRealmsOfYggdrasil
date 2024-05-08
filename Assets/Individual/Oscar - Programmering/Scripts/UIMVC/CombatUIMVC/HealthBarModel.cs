using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarModel : MonoBehaviour
{
    public Image imageRenderer;
    public TMP_Text healthText;
    public List<Sprite> sprites;
    
    public int currentHealth;
    public int maxHealth;
    [NonSerialized]public List<float> levels;
}

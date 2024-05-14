using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : ElementMVC
{
    

    public Image imageRenderer;
    public TMP_Text healthText;
    public List<Sprite> sprites;
    
    
    public void Awake()
    {
        app.Notify(NotificationMVC.HealthBarLevelsSetView, this, sprites);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : BaseControllerMVC
{
   
    public void Awake()
    {
        LevelsSet();
    }
    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        switch (p_event_path)
        {
               
            case NotificationMVC.SetHealth:
            {
                PlayerHealthData healthData = (PlayerHealthData)p_data[0];
                app.model.healthBarModel.currentHealth = (int)healthData.currentHealth;
                app.model.healthBarModel.maxHealth = (int) healthData.maxHealth;
                app.model.healthBarModel.healthText.text = app.model.healthBarModel.currentHealth.ToString();
                SetCurrentSprite();
                break;
            }
        }
    }
 
    
    public void SetCurrentSprite()
    {
        //Makes it so the health is always in a span of 0 to 1.
        if (app.model.healthBarModel.maxHealth != 0)
        {
            var currentHealthNormalized = app.model.healthBarModel.currentHealth / (float)app.model.healthBarModel.maxHealth;
          

            for (int i = app.model.healthBarModel.levels.Count-1; i >= 0; i--)
            {
                if (currentHealthNormalized <= app.model.healthBarModel.levels[i])
                {
                    app.model.healthBarModel.imageRenderer.sprite = app.model.healthBarModel.sprites[i];
                }
            }
        }
        
    }
    
    private void LevelsSet()
    {
        app.model.healthBarModel.levels = new List<float>();
        for (int i = 0; i < app.model.healthBarModel.sprites.Count; i++)
        {
            app.model.healthBarModel.levels.Add((float)i / (app.model.healthBarModel.sprites.Count-1));
        }
    }
}

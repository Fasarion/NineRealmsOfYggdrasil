using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : BaseControllerMVC
{
   
    /*public void Awake()
    {
        LevelsSet();
    }*/
    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        var healthBarView = app.view.healthBarView;
        var healthBarModel = app.model.healthBarModel;
        
        switch (p_event_path)
        {
               
            case NotificationMVC.SetHealthModel:
            {
                healthBarView.healthText.text = healthBarModel.currentHealth.ToString();
                SetCurrentSprite(healthBarView, healthBarModel);
                break;
            }
            case NotificationMVC.HealthBarLevelsSetView:
            {
                healthBarModel.levels = new List<float>();
                for (int i = 0; i < healthBarView.sprites.Count; i++)
                {
                    healthBarModel.levels.Add((float)i / (healthBarView.sprites.Count-1));
                }

                break;
            }
        }
    }
 
    
    public void SetCurrentSprite(HealthBarView healthBarView, HealthBarModel healthBarModel)
    {
        //Makes it so the health is always in a span of 0 to 1.
        if (healthBarModel.maxHealth != 0)
        {
            var currentHealthNormalized = healthBarModel.currentHealth / (float)healthBarModel.maxHealth;
          

            for (int i = healthBarModel.levels.Count-1; i >= 0; i--)
            {
                if (currentHealthNormalized <= healthBarModel.levels[i])
                {
                    healthBarView.imageRenderer.sprite = healthBarView.sprites[i];
                }
            }
        }
        
    }
}

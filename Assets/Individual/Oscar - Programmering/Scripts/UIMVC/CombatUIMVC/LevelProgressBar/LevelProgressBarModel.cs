using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressBarModel : ElementMVC
{
    public void OnEnable()
    {
        EventManager.OnPlayerExperienceChange += OnPlayerExperienceChange;
    }
    
    public void OnDisable()
    {
        EventManager.OnPlayerExperienceChange -= OnPlayerExperienceChange;
    }
    
    
    public void OnPlayerExperienceChange(ExperienceInfo experienceInfo)
    {
        //ExperienceInfo acts as the model in this case.
        app.Notify(NotificationMVC.ExperienceInfoChanged,this, experienceInfo);
        
    }
}

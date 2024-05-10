using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBarController : BaseControllerMVC
{
    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {

        var progressBarView = app.view.progressBarView;
        var progressBarModel = app.model.progressBarModel;
        switch (p_event_path)
        {
            case NotificationMVC.ExperienceInfoChanged:
            {
                ExperienceInfo experienceInfo = (ExperienceInfo)p_data[0];
                progressBarView.slider.value = experienceInfo.currentXP;
                progressBarView.slider.maxValue = experienceInfo.experienceNeededToLevelUp;
                progressBarView.levelText.text = experienceInfo.currentLevel.ToString();
                break;
            }
        }
    }
}

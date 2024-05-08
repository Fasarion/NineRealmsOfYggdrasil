using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashingBoxController : BaseControllerMVC
{
    
    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        switch (p_event_path)
        {
            case NotificationMVC.SetFlashingBoxColor:
            {
                
                app.flashingBoxModel.boxColor = (Color)p_data[0];
                var image = (Image)p_data[1];
                image.color = app.flashingBoxModel.boxColor;
                Debug.Log("ColorSwapped" + app.flashingBoxModel.boxColor);
                break;
            }
        }
    }
}

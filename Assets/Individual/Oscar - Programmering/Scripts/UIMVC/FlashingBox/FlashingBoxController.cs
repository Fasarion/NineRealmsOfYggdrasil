using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class FlashingBoxController : BaseControllerMVC
{
    
    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        switch (p_event_path)
        {
            case NotificationMVC.SetFlashingBoxColor:
            {
                
                app.model.boxModel.boxColor = (Color)p_data[0];
                var image = (Image)p_data[1];
                image.color = app.model.boxModel.boxColor;
                Debug.Log("ColorSwapped" + app.model.boxModel.boxColor);
                break;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashingBoxView : ElementMVC
{
    public Image image;
    public Color color;
    public void SetView()
    {
        app.Notify(NotificationMVC.SetFlashingBoxColor, this, color, image);
    }
}

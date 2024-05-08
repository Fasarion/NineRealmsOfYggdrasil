using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashingBoxView : ElementMVC
{
    public Image image;
    public Color color;
    private object[] objectArray;
    public int number1;
    public string string1;
    public void SetView()
    {
        app.Notify(NotificationMVC.SetFlashingBoxColor, this, color, image);
    }
}

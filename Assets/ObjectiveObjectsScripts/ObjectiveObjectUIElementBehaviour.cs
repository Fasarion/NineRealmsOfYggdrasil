using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ObjectiveObjectUIElementBehaviour : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI text;

    public void PopulateUI(Sprite sprite, string count)
    {
        if(sprite != null) image.sprite = sprite;
        text.text = count;
    }
}

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

    public void PopulateUI(Sprite sprite, int count)
    {
        image.sprite = sprite;
        text.text = count.ToString();
    }
}

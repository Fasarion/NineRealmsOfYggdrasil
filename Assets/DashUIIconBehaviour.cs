using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DashUIIconBehaviour : MonoBehaviour
{
    [Header("Fill")]
    [SerializeField] private Image fillImage;
    
    [Header("Dash")]
    [SerializeField] private Image dashIcon;
    
    [Header("Color")]
    [SerializeField] private float alphaWhenNotReady = 0.5f;
    
    public void UpdateInfo(DashInfoElement dashInfoElement)
    {
        var dashInfo = dashInfoElement.Value;
        
        bool ready = dashInfo.Ready;

        fillImage.fillAmount = dashInfo.CurrentTime / dashInfo.CooldownTime;

        float alpha = ready ? 1 : alphaWhenNotReady;

        UpdateImageColor(dashIcon, alpha);
        UpdateImageColor(fillImage, alpha);
    }

    private void UpdateImageColor(Image image, float alpha)
    {
        var oldColor = image.color;
        image.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
    }
}
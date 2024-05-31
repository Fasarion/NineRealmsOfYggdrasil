using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DashUIIconBehaviour : MonoBehaviour
{
    [Header("Contents")] 
    [SerializeField] private GameObject dashContents;

    [Header("Fill")]
    [SerializeField] private Image fillImage;
    
    [Header("Dash")]
    [SerializeField] private Image dashIcon;
    
    [Header("Color")]
    [SerializeField] private float alphaWhenNotReady = 0.5f;
    
    public void UpdateInfo(DashInfoElement dashInfoElement)
    {
        var dashInfo = dashInfoElement.Value;
        
        float tVal = dashInfo.CurrentTime / dashInfo.CooldownTime;
        
        fillImage.fillAmount = tVal;


        UpdateImageColor(dashIcon,  tVal);
        UpdateImageColor(fillImage, tVal);
    }

    private void UpdateImageColor(Image image, float t)
    {
        float alpha;

        if (t >= 1)
        {
            alpha = 1;
        } 
        else if (t <= 0)
        {
            alpha = 0;
        }
        else
        {
            alpha = alphaWhenNotReady;
        }
        
        dashContents.SetActive(alpha > 0);

        var oldColor = image.color;
        image.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
    }
}
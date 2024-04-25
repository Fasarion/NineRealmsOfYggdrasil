using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressIndicatorBackgroundUI : MonoBehaviour
{
    
    private RectTransform rectTransform;
    //private RectTransform parentRectTransform;
    public RectTransform runtimeParentRectTransform;
    
    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        //parentRectTransform = transform.parent.GetComponent<RectTransform>();
        //UpdateProgressBackgroundXPosition();
    }

    
    public void UpdateProgressBackgroundXPosition()
    {
        var runtimeParentRect = runtimeParentRectTransform.rect;
        
        //float parentWidth = parentRect.width;
        
        // Get the right stretch parameter of the RectTransform
        float right = -(rectTransform.offsetMax.x);
        
        rectTransform.offsetMax = new Vector2(  (runtimeParentRectTransform.anchoredPosition.x + (runtimeParentRect.width/2)), rectTransform.offsetMax.y);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

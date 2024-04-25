using UnityEngine;
using UnityEngine.UI;

public class CustomHorizontalLayoutGroup : HorizontalLayoutGroup
{
    public delegate void LayoutRebuiltCallback();
    public LayoutRebuiltCallback onLayoutRebuilt;

    public override void SetLayoutHorizontal()
    {
        base.SetLayoutHorizontal();
        
        // Call the callback when layout is rebuilt
        onLayoutRebuilt?.Invoke();
    }
}

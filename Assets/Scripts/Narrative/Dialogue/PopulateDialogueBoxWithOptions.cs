using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateDialogueBoxWithOptions : MonoBehaviour
{
    // Start is called before the first frame update
    public Button buttonPrefab;
    private RectTransform rectTransform;
    void Start()
    {
      
        rectTransform = GetComponent<RectTransform>();  
        PopulateDialogueWindow();
        
    }

    // Update is called once per frame
    void PopulateDialogueWindow()
    {
        var button = Instantiate(buttonPrefab, transform);
        button.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, 0);
        
    }
}

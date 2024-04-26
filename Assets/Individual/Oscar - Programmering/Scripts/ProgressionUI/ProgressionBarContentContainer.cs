using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class ProgressionBarContentContainer : MonoBehaviour
{
    //public List<ProgressionBarLevelContainer> levelContainers;
    
    public Transform levelsRuntimeParent;
    public Transform symbolsParent;
    
    public static Action<ProgressionBarLevelContainer[]> onProgressionContentSet;
    public float contentWidth;
 
    private CustomHorizontalLayoutGroup horizontalLayoutGroup;

    private bool rectBoundsSet = false;
    public static Action<RectTransform> onRectBoundsSet;
    private ProgressionBarLevelContainer[] levelContainerArray;
    
    public void Awake()
    {
        //levelContainers = new List<ProgressionBarLevelContainer>();
        horizontalLayoutGroup = GetComponentInChildren<CustomHorizontalLayoutGroup>();
        rectBoundsSet = false;
    }

    void Start()
    { 
        levelContainerArray =  transform.GetComponentsInChildren<ProgressionBarLevelContainer>();
        if (levelContainerArray == null || levelContainerArray.Length == 0)
        {
            Debug.LogError("List of possible levels for the room tree generator did not exist in the content container on the UI canvas");
        }
        else
        {
            for (int i = 0; i < levelContainerArray.Length; i++)
            {
                levelContainerArray[i].SetProgressionBarContentContainerAsParent(symbolsParent);
                levelContainerArray[i].transform.parent = levelsRuntimeParent;
            }
            
            onProgressionContentSet?.Invoke(levelContainerArray);
        }
    }

    
    private void OnEnable()
    {
        horizontalLayoutGroup.onLayoutRebuilt += OnLayoutRebuilt;
        //(geometryChangedEvent.  += OnGeometryChangedEvent;
        // Subscribe to the Rebuilt event of the element's LayoutRebuilder


    }

    private void OnLayoutRebuilt()
    {
        if (rectBoundsSet == false)
        {
            var rectTransform = GetComponent<RectTransform>();
            var rect = rectTransform.rect;
            rectTransform.ForceUpdateRectTransforms();
            var sizeDelta = rectTransform.sizeDelta;
            contentWidth = rect.width;
            rectBoundsSet = true;
            OnRectBoundsSet(rect);
            onRectBoundsSet?.Invoke(rectTransform);
            

            //Debug.Log("Rebuilt!");
        }
       
    }
    

    public void OnRectBoundsSet(Rect rect)
    {
        for (int i = 0; i < levelContainerArray.Length; i++)
        {
            for (int j = 0; j < levelContainerArray[i].symbolBehaviours.Length; j++)
            {
                levelContainerArray[i].symbolBehaviours[j].SetXPosition();
            }
        }
    }

    private void OnDisable()
    {
        horizontalLayoutGroup.onLayoutRebuilt -= OnLayoutRebuilt;
        // Unsubscribe from the Rebuilt event
       
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < levelContainerArray.Length; i++)
        {
            for (int j = 0; j < levelContainerArray[i].symbolBehaviours.Length; j++)
            {
                levelContainerArray[i].symbolBehaviours[j].SetXPosition();
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class MapProgressionBarScroller : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField]  private float scrollRate = 1.5f;
    [SerializeField]   private float lerpDuration = 0.1f;
    // Start is called before the first frame update
    //public Button leftScrollButton;
    //public Button rightScrollButton;
    
    private float scrollStart;
    private float scrollFinished;
    private float currentTime;
    private AudioManager audioManager;
  
    void Start()
    {
        if (!audioManager)
        {
            audioManager = AudioManager.Instance;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime/lerpDuration < 1)
        {
            currentTime += Time.deltaTime;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollStart, scrollFinished, currentTime);
        }
        //scrollRect.horizontalNormalizedPosition -= 0.1f * Time.deltaTime;
        
    }

    public void OnClickRight()
    {
        
        ScrollLeft();
        audioManager.uiAudio.MenuClickAudio();
    }

    public void OnClickLeft()
    {
        
        ScrollRight();
        audioManager.uiAudio.MenuClickAudio();
    }
    //Used by 
    public void ScrollRight()
    {
        
        scrollStart = scrollRect.horizontalNormalizedPosition;
        scrollFinished = scrollRect.horizontalNormalizedPosition + scrollRate;
        currentTime = 0;

    }

    public void ScrollLeft()
    {
        scrollStart = scrollRect.horizontalNormalizedPosition;
        scrollFinished = scrollRect.horizontalNormalizedPosition - scrollRate;
        currentTime = 0;
    }
}

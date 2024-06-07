using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSegment : MonoBehaviour
{
    private TutorialManager tutorialManager;
    public bool tutorialActive = false;
    public virtual void Start()
    {
        tutorialManager = FindObjectOfType<TutorialManager>();
       
    }

    public virtual void StartSegment()
    {
        tutorialActive = true;
    }
    public void SegmentCompleted()
    {
        tutorialManager.MoveToNextSegment();
    }

  
}

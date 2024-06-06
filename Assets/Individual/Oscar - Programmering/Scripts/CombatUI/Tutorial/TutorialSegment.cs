using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSegment : MonoBehaviour
{
    private TutorialManager tutorialManager;

    public virtual void Start()
    {
        tutorialManager = FindObjectOfType<TutorialManager>();
    }

    public virtual void StartSegment()
    {
        
    }
    public void SegmentCompleted()
    {
        tutorialManager.MoveToNextSegment();
    }

  
}

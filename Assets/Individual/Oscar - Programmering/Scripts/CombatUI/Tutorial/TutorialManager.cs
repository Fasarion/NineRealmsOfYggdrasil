using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public List<TutorialSegment> tutorialSegments;
    private int segmentCounter;

    private TutorialSegment currentSegment;

    public void Start()
    {
        segmentCounter = 0;
        if (tutorialSegments != null && tutorialSegments.Count != 0)
        {
            currentSegment = tutorialSegments[0];
            currentSegment.StartSegment();
        }
        
    }

    public void MoveToNextSegment()
    {
        segmentCounter++;
        if (segmentCounter < tutorialSegments.Count)
        {
            currentSegment = tutorialSegments[segmentCounter];
            currentSegment.StartSegment();
        }
        else
        {
            //Debug.Log("Tutorial Complete!");
        }
        
    }
   

    // Update is called once per frame
    void Update()
    {
        
    }
}

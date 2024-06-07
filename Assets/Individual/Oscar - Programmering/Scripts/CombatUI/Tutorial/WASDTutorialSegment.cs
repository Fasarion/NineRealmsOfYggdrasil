using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASDTutorialSegment : TutorialSegment
{
    public LanguageLocalizerBehaviour localizerBehaviour;
    public bool hasPressedUp;
    public bool hasPressedDown;
    public bool hasPressedLeft;
    public bool hasPressedRight;
    
    public bool hasCompletedSegment;

    public GameObject WBorder;
    public GameObject ABorder;
    public GameObject SBorder;
    public GameObject DBorder;
    public GameObject WASDText;
    public GameObject WASDSWEText;
    public bool useSwedish;

    
    public void OnEnable()
    {
        EventManager.OnPlayerMoveInput += OnPlayerMoveInput;
    }
    
    public void OnDisable()
    {
        EventManager.OnPlayerMoveInput -= OnPlayerMoveInput;
    }

    private void OnPlayerMoveInput(Vector3 moveVector)
    {
        
        if (moveVector.z > 0)
        {
            hasPressedUp = true;
        }

        if (moveVector.z < 0)
        {
            hasPressedDown = true;
        }

        if (moveVector.x > 0)
        {
            hasPressedRight = true;
        }

        if (moveVector.x < 0)
        {
            hasPressedLeft = true;
        }
        
        
        if (tutorialActive)
        {
            if (hasPressedUp)
            {
                WBorder.SetActive(false);
            }
            
            if (hasPressedDown)
            {
                SBorder.SetActive(false);
            }
            
            if (hasPressedRight)
            {
                DBorder.SetActive(false);
            }
            
            if (hasPressedLeft)
            {
                ABorder.SetActive(false);
            }

            if (hasPressedUp && hasPressedDown && hasPressedLeft && hasPressedRight)
            {
                hasCompletedSegment = true;
                tutorialActive = false;
                if (!useSwedish)
                {
                    WASDText.SetActive(false);
                }
                else
                {
                    WASDSWEText.SetActive(false);
                }
               
               
                SegmentCompleted();
            }
        }
        
    }

    public override void StartSegment()
    {
        useSwedish = localizerBehaviour.GetLanguage();
        base.StartSegment();
        tutorialActive = true;
        if(!useSwedish)
        {
            WASDText.SetActive(true);
        }
        else
        {
            WASDSWEText.SetActive(true);
        }
      
        WBorder.SetActive(true);
        ABorder.SetActive(true);
        SBorder.SetActive(true);
        DBorder.SetActive(true);
        
    }
    public override void  Start()
    {
        base.Start();
        hasPressedUp = false; 
        hasPressedDown = false; 
        hasPressedLeft = false; 
        hasPressedRight = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

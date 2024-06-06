using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASDTutorialSegment : TutorialSegment
{
    
    public bool hasPressedUp;
    public bool hasPressedDown;
    public bool hasPressedLeft;
    public bool hasPressedRight;

    public GameObject WBorder;
    public GameObject ABorder;
    public GameObject SBorder;
    public GameObject DBorder;
    public GameObject WASDText;

    
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
            WBorder.SetActive(false);
            
        }
        
        if (moveVector.z < 0)
        {
            hasPressedDown = true;
           
            SBorder.SetActive(false);
        }
        
        if (moveVector.x > 0)
        {
            hasPressedRight = true;
            DBorder.SetActive(false);
           
        }

        if (moveVector.x < 0)
        {
            hasPressedLeft = true;
            ABorder.SetActive(false);
            
        }

        if (hasPressedUp && hasPressedDown && hasPressedLeft && hasPressedRight)
        {
            EventManager.OnPlayerMoveInput -= OnPlayerMoveInput; 
            WASDText.SetActive(false);
            SegmentCompleted();
        }
    }

    public override void StartSegment()
    {
        WASDText.SetActive(true);
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

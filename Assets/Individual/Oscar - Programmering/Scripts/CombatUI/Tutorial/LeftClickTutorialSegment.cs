using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class LeftClickTutorialSegment : TutorialSegment
{
    public GameObject leftClickHighlight;
    //public bool LeftClickTutorialActive;

    public void OnEnable()
    {
        EventManager.OnUpdateAttackAnimation += OnLeftClick;
    }
    public void OnDisable()
    {
        EventManager.OnUpdateAttackAnimation -= OnLeftClick;
    }

    public override void Start()
    {
        base.Start();
        
    }
    public override void StartSegment()
    {
        base.StartSegment();
        leftClickHighlight.SetActive(true);
    }
    private void OnLeftClick(AttackType attackType,bool canNormalAttack)
    {
        if (tutorialActive)
        {
            if (attackType == AttackType.Normal && canNormalAttack)
            { 
                leftClickHighlight.SetActive(false);
                tutorialActive = false;
                SegmentCompleted();
            }
        }
      
    }

    

    // Start is called before the first frame update
   

    // Update is called once per frame
    void Update()
    {
        
    }
}

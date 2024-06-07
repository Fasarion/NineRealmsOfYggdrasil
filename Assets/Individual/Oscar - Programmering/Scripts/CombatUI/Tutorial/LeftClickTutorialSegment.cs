using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class LeftClickTutorialSegment : TutorialSegment
{
    public bool useSwedish;
    public LanguageLocalizerBehaviour localizerBehaviour;
    public GameObject leftClickHighlight;
    public GameObject leftClickHighlightSWE;
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
        useSwedish = localizerBehaviour.GetLanguage();
        
    }
    public override void StartSegment()
    {
        useSwedish = localizerBehaviour.GetLanguage();
        base.StartSegment();
        if (!useSwedish)
        {
            leftClickHighlight.SetActive(true);
        }
        else
        {
            leftClickHighlightSWE.SetActive(true);
        }
        
    }
    private void OnLeftClick(AttackType attackType,bool canNormalAttack)
    {
        if (tutorialActive)
        {
            if (attackType == AttackType.Normal && canNormalAttack)
            {
                if (!useSwedish)
                {
                    leftClickHighlight.SetActive(false);
                }
                else
                {
                    leftClickHighlightSWE.SetActive(false);
                }
               
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTutorialSegment : TutorialSegment
{
    public bool useSwedish;
    public GameObject dashTutorialObject;
    public GameObject dashTutorialObjectSWE;
    public LanguageLocalizerBehaviour localizerBehaviour;
    public override void Start()
    {
        base.Start();
        useSwedish = localizerBehaviour.GetLanguage();
        if (!useSwedish)
        {
            dashTutorialObject.SetActive(false);
        }
        else
        {
            dashTutorialObjectSWE.SetActive(false);
        }
    }
    public override void StartSegment()
    {
        base.StartSegment();
        useSwedish = localizerBehaviour.GetLanguage();
        if (!useSwedish)
        {
            dashTutorialObject.SetActive(true);
        }
        else
        {
            dashTutorialObjectSWE.SetActive(true);
        }
    }

    public void OnEnable()
    {
        EventManager.OnDashInput += OnDash;
    }

    public void OnDisable()
    {
        EventManager.OnDashInput -= OnDash;
    }
    private void OnDash()
    {
        if (tutorialActive)
        {
            if (!useSwedish)
            {
                dashTutorialObject.SetActive(false);
            }
            else
            {
                dashTutorialObjectSWE.SetActive(false);
            }
            
            SegmentCompleted();
            tutorialActive = false;
        }
    }
}

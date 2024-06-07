using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTutorialSegment : TutorialSegment
{
    public GameObject dashTutorialObject;

    public override void Start()
    {
        base.Start();
        dashTutorialObject.SetActive(false);
    }
    public override void StartSegment()
    {
        base.StartSegment();
        dashTutorialObject.SetActive(true);
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
            dashTutorialObject.SetActive(false);
            SegmentCompleted();
            tutorialActive = false;
        }
    }
}

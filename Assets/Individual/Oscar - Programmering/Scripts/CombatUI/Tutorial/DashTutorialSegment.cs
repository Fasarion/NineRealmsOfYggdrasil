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
        dashTutorialObject.SetActive(true);
    }

    public void OnEnable()
    {
        EventManager.OnDashInput += OnDash;
    }

    private void OnDash()
    {
        dashTutorialObject.SetActive(false);
        SegmentCompleted();
    }
}

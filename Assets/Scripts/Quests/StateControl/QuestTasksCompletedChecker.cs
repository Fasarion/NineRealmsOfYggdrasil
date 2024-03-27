using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTasksCompletedChecker : MonoBehaviour
{
    public QuestTrigger trigger;
    public int stateToSwapTo;

    public void OnEnable()
    {
        trigger.handler.onQuestTasksCompleted += OnQuestTasksCompleted;
    }
    
    public void OnDisable()
    {
        trigger.handler.onQuestTasksCompleted -= OnQuestTasksCompleted;
    }

    public void OnQuestTasksCompleted()
    {
        trigger.handler.SetCurrentQuestState(stateToSwapTo);
        trigger.RunQuestTriggersOnCurrentState();
    }
}

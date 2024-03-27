using System;
using System.Collections;
using System.Collections.Generic;
using KKD;
using UnityEngine;

public class QuestCompleteChecker : MonoBehaviour
{
    [SerializeField]private QuestTrigger questTrigger;
    //[SerializeField]private GameObject questRewardObjectNotImplemented;
    [SerializeField] private List<QuestHandler>  newQuestsToActivate;

    public void OnEnable()
    {
        questTrigger.QuestCompleted();

        if (newQuestsToActivate.Count == 0)
        {
            for (int i = 0; i < newQuestsToActivate.Count; i++)
            {
                newQuestsToActivate[i].QuestActivated();
            }
        }
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using KKD;
using UnityEngine;



public class QuestTrigger : MonoBehaviour
{
    
    public QuestHandler handler;
    public QuestUIUpdate questUIUpdate;

    public List<QuestStates> questStatesList;
    //public List<GameObject> objectsToActivate;
    //public List<GameObject> objectsToDeactivate;
    public List<GameObject> questObjects;
    private void Awake()
    {
        questUIUpdate = FindObjectOfType<QuestUIUpdate>();
    }

    private void Start()
    {
        if (handler.questAccepted)
        {
            questUIUpdate.AddQuestPanel(handler);
        }
        RunQuestTriggersOnCurrentState();
        
        

    }

    public void QuestAccepted()
    {
        if (handler.firstTimeAccepting)
        {
            handler.questAccepted = true;
            handler.firstTimeAccepting = false;
            //Would be nice if this one triggered only if you didn't already have the quest.
            questUIUpdate.AddQuestPanel(handler);
        }
        
    }

    public void QuestCompleted()
    {
        handler.QuestCompleted(handler);
    }
    

    public void RunQuestTriggersOnCurrentState()
    {
        for (int i = 1; i < questObjects.Count; i++)
        {
            questObjects[i].SetActive(false);
        }
        var currentQuestState = handler.GetCurrentQuestState();
        
        if (questStatesList.Count > currentQuestState)
        {
            for (int i = 0; i < questStatesList[currentQuestState].objectsToActivate.Count; i++)
            {
                questStatesList[currentQuestState].objectsToActivate[i].SetActive(true);
            }
        
            /*for (int i = 0; i < questStatesList[currentQuestState].objectsToDeactivate.Count; i++)
            {
                questStatesList[currentQuestState].objectsToDeactivate[i].SetActive(false);
            }*/
        }
        
        
    }
}

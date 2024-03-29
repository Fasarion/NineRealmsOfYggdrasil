using System;
using System.Collections;
using System.Collections.Generic;
using KKD;
using Unity.VisualScripting;
using UnityEngine;

public class SoResetterBehaviour : MonoBehaviour
{
    
    public List<QuestHandler> questHandlers;

    public bool resetQuest;

    private void Awake()
    {
       
        
        
        
        if (resetQuest)
        {
            foreach (var questHandler in questHandlers)
            {
                questHandler.ResetKills();
                questHandler.ResetItemCollect();
                questHandler.ResetFetch();
                questHandler.ResetQuestState();
                questHandler.questAccepted = false;
                questHandler.questActive = false;
                questHandler.questComplete = false;
            }
        }
       
        

    }


    
    
}

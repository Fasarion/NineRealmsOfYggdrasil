using System.Collections;
using System.Collections.Generic;
using KKD;
using QS.Enumerations;
using UnityEngine;

public class QSQuestHandlerSO : QSQuestSO
{
    [field: SerializeField] public QuestHandler QuestHandler { get; set; }

    public void Initialize(string nodeName,List<QSQuestBranchData> branches, QuestHandler questHandler, QSQuestNodeType questNodeType, bool isStartingNode)
    {
        base.Initialize(nodeName,branches, questNodeType,isStartingNode);
        QuestHandler = questHandler;
        
    }
}

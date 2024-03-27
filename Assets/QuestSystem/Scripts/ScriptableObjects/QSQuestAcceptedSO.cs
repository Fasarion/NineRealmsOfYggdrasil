using System.Collections;
using System.Collections.Generic;
using QS.Enumerations;
using UnityEngine;

public class QSQuestAcceptedSO : QSQuestSO
{
    public override void Initialize(string nodeName, List<QSQuestBranchData> branches, QSQuestNodeType questNodeType, bool isStartingNode)
    {
        base.Initialize(nodeName, branches, questNodeType, isStartingNode);
        
        
    }
}

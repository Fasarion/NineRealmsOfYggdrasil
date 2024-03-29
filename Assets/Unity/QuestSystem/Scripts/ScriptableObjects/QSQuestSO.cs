using System.Collections;
using System.Collections.Generic;
using QS.Enumerations;
using UnityEngine;

public class QSQuestSO : ScriptableObject
{
    [field: SerializeField] public string NodeName { get; set; }
    [field: SerializeField] private QSQuestNodeType QuestNodeType { get; set; }
    [field: SerializeField] private bool IsStartingNode { get; set; }
    [field: SerializeField] public List<QSQuestBranchData> Branches { get; set; }

    public virtual void Initialize(string nodeName,List<QSQuestBranchData> branches, QSQuestNodeType questNodeType, bool isStartingNode)
    {
        NodeName = nodeName;
        QuestNodeType = questNodeType;
        IsStartingNode = isStartingNode;
        Branches = branches;
    }
}

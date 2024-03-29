using System.Collections;
using System.Collections.Generic;
using DS.Data.Save;
using QS.Data.Save;
using QS.Elements;
using UnityEngine;

public class QSGraphSaveDataSO : ScriptableObject
{
    [field: SerializeField] public string FileName { get; set; }
    [field: SerializeReference] public List<QSNodeSaveData> Nodes { get; set; }
    [field: SerializeField] public List<QSQuestHandlerNodeSaveData> QuestHandlerNodes { get; set; }
    [field: SerializeField] public List<QSDialogueGraphNodeSaveData> DialogueGraphNodes { get; set; }
    [field: SerializeField] public List<QSActivatorNodeSaveData> ActivatorNodes { get; set; }
    [field: SerializeField] public List<QSConditionNodeSaveData> ConditionNodes { get; set; }
    [field: SerializeField] public List<QSConditionSetterNodeSaveData> ConditionSetterNodes { get; set; }
    
    [field: SerializeField] public List<QSQuestAcceptedNodeSaveData> QuestAcceptedNodes { get; set; }
    
    [field: SerializeField] public List<string> OldNodeNames { get; set; }
    [field: SerializeField] public List<string> OldQuestHandlerNodeNames { get; set; }
    [field: SerializeField] public List<string> OldDialogueGraphNodeNames { get; set; }
    [field: SerializeField] public List<string> OldActivatorNodeNames { get; set; }
    [field: SerializeField] public List<string> OldConditionNodeNames { get; set; }
    [field: SerializeField] public List<string> OldQuestAcceptedNodeNames { get; set; }
    [field: SerializeField] public List<string> OldQuestActivatorNodeNames { get; set; }
    [field: SerializeField] public List<string> OldConditionSetterNodeNames { get; set; }


    public void Initialize(string fileName)
    {
        FileName = fileName;
        Nodes = new List<QSNodeSaveData>();
        QuestHandlerNodes = new List<QSQuestHandlerNodeSaveData>();
        DialogueGraphNodes = new List<QSDialogueGraphNodeSaveData>();
        ActivatorNodes = new List<QSActivatorNodeSaveData>();
        ConditionNodes = new List<QSConditionNodeSaveData>();
        QuestAcceptedNodes = new List<QSQuestAcceptedNodeSaveData>();
        ConditionSetterNodes = new List<QSConditionSetterNodeSaveData>();
        //OldNodeNames = new List<string>();
        //OldQuestNodeNames = new List<string>();
        //OldDialogueGraphNodeNames = new List<string>();
        //OldActivatorNodeNames = new List<string>();

    }
}

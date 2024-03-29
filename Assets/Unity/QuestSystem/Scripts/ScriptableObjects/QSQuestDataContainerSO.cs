using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class QSQuestDataContainerSO : ScriptableObject
{
    [field: SerializeField] public string FileName { get; set; }
    //[field: SerializeField] public List<>
    [field: SerializeField] public List<QSDialogueGraphSO> questDialogueGraphSOs { get; set; }
    [field: SerializeField] public List<QSActivatorSO> activatorSOs { get; set; }
    [field: SerializeField] public List<QSQuestHandlerSO> questHandlerSOs { get; set; }
    [field: SerializeField] public QSQuestSO startingNode { get; set; }

    public void Initialize(string fileName)
    {
        FileName = fileName;
        questDialogueGraphSOs = new List<QSDialogueGraphSO>();
        activatorSOs = new List<QSActivatorSO>();
        questHandlerSOs = new List<QSQuestHandlerSO>();
    }
    
    
}

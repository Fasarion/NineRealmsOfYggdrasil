using System;
using System.Collections;
using System.Collections.Generic;
using QS.Enumerations;
using UnityEngine;

namespace QS.Data.Save
{
    [Serializable]
    public class QSNodeSaveData
    {
        [field: SerializeField]public string ID { get; set; }
        [field: SerializeField]public string Name { get; set; }
        [field: SerializeField]public List<QSBranchSaveData> Branches { get; set; }
        [field: SerializeField]public QSQuestNodeType QuestNodeType { get; set; }
        [field: SerializeField]public Vector2 Position { get; set; }
        
    }
}
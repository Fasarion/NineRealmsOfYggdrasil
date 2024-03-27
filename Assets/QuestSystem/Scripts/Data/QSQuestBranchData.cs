using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QSQuestBranchData
{
    [field: SerializeField] public string Text { get; set; }
    [field: SerializeField] public QSQuestSO NextQuestNode { get; set; }
}

using System;
using System.Collections;
using System.Collections.Generic;
using KKD;
using QS.Data.Save;
using UnityEngine;

[Serializable]
public class QSQuestActivatorNodeSaveData : QSNodeSaveData
{
    [field: SerializeField]public QuestHandler QuestHandler { get; set; }
}

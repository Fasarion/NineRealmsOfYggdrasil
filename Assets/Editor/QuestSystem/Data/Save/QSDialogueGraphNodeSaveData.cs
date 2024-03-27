using System;
using DS.ScriptableObjects;
using UnityEngine;


namespace QS.Data.Save
{
    [Serializable]
    public class QSDialogueGraphNodeSaveData : QSNodeSaveData
    {
        [field: SerializeField] public DSDialogueContainerSO DialogueContainerSO;
    }
}
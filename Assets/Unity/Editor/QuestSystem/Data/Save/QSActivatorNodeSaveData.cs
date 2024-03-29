using System.Collections;
using System.Collections.Generic;
using QS.Data.Save;
using UnityEngine;

public class QSActivatorNodeSaveData : QSNodeSaveData
{
    [field: SerializeReference]public List<string> GameObjectsToActivateNames;
    [field: SerializeReference]public List<string> GameObjectToDeactivateNames;
}

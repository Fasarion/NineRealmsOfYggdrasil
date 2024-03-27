using System.Collections;
using System.Collections.Generic;
using QS.Enumerations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class QSActivatorSO : QSQuestSO
{
    [field: SerializeField] public List<string> GameObjectsToActivateNames { get; set; }
    [field: SerializeField] public List<string> GameObjectsToDeactivateNames { get; set; }

    //Store data for activators here.
    public void Initialize(string nodeName,List<QSQuestBranchData> branches, List<string> gameObjectsToActivateNames, List<string> gameObjectsToDeactivateNames, QSQuestNodeType questNodeType, bool isStartingNode)
    {
        base.Initialize(nodeName,branches,questNodeType,isStartingNode);
        GameObjectsToActivateNames = gameObjectsToActivateNames;
        GameObjectsToDeactivateNames = gameObjectsToDeactivateNames;
       
    }
    
}

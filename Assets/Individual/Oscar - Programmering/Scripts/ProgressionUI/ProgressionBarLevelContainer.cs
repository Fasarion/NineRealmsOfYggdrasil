using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionBarLevelContainer : MonoBehaviour
{
    public Transform[] childTransformArray;
    public ProgressionBarBehaviour[] symbolBehaviours;
    public void Awake()
    {
        AddChildrenToList();
    }

    public void AddChildrenToList()
    { 
        childTransformArray =  transform.GetComponentsInChildren<Transform>();
        symbolBehaviours =  transform.GetComponentsInChildren<ProgressionBarBehaviour>();
    }

    public void SetProgressionBarContentContainerAsParent(Transform parent)
    {
        foreach (var progressionBarBehaviour in symbolBehaviours)
        {
            progressionBarBehaviour.transform.parent = parent;
        }
    }
}

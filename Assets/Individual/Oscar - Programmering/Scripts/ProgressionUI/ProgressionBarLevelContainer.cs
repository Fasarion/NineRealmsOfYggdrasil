using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionBarLevelContainer : MonoBehaviour
{
    public Transform[] childTransformArray;
    public void Awake()
    {
        AddChildrenToList();
    }

    public void AddChildrenToList()
    { 
        childTransformArray =  transform.GetComponentsInChildren<Transform>();
    }
}

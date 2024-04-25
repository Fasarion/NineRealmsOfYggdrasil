using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ProgressionBarContentContainer : MonoBehaviour
{
    //public List<ProgressionBarLevelContainer> levelContainers;

    public Transform levelsRuntimeParent;

    public static Action<ProgressionBarLevelContainer[]> onProgressionContentSet;
    public void Awake()
    {
        //levelContainers = new List<ProgressionBarLevelContainer>();
    }

    void Start()
    { 
        var levelContainerArray =  transform.GetComponentsInChildren<ProgressionBarLevelContainer>();
        if (levelContainerArray == null || levelContainerArray.Length == 0)
        {
            Debug.LogError("List of possible levels for the room tree generator did not exist in the content container on the UI canvas");
        }
        else
        {
            for (int i = 0; i < levelContainerArray.Length; i++)
            {
                levelContainerArray[i].SetProgressionBarContentContainerAsParent(transform);
                levelContainerArray[i].transform.parent = levelsRuntimeParent;
            }
            
          
            
            onProgressionContentSet?.Invoke(levelContainerArray);
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

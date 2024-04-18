using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ProgressionBarContentContainer : MonoBehaviour
{

    public List<ProgressionBarLevelContainer> levelContainers;


    public static Action<int> onProgressionContentSet;
    // Start is called before the first frame update
    public void Awake()
    {
        levelContainers = new List<ProgressionBarLevelContainer>();
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
           onProgressionContentSet.Invoke(levelContainerArray.Length);
       }
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using DS;
using DS.ScriptableObjects;
using UnityEngine;

public class DialogueContainerGeneratorBehaviour : MonoBehaviour
{
    private GraphToManagerConnector connector;

    public List<DSDialogueContainerSO> DSContainers;

    public DialogueContainer dialogueContainerPrefab;
    public ContainerParent dialogueContainerParent;
    
    public Dictionary<string,ContainerParent> dialogueContainerParents;
    private bool initialized = false;

    //private  DSNodeSaveData currentNode;
    // Start is called before the first frame update
    public void Awake()
    {
        /*if (!initialized)
        {
            connector = new GraphToManagerConnector(dialogueContainerPrefab, dialogueContainerParent);
            dialogueContainerParents = new Dictionary<string,ContainerParent>();
            initialized = true;
        }*/
       
    }

    public void Initialize()
    {
        if (!initialized)
        {
            connector = new GraphToManagerConnector(dialogueContainerPrefab, dialogueContainerParent);
            dialogueContainerParents = new Dictionary<string,ContainerParent>();
            dialogueContainerParents = connector.AddContainerGraphsToList(DSContainers, dialogueContainerPrefab);
            initialized = true;
        }
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using DS.ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public struct DictionaryDisplay {
    public string name;
    public DialogueContainer container;
}

public enum ContainerType
{
    DialogueOnly,
    DialogueWithStateSwapper,
    BranchingDialogue,
    StateSwapper,
    
}
public class DialogueContainer : MonoBehaviour//, ISerializationCallbackReceiver
{
    //Dialogue System Graph specific parameters
    public DSDialogueSO dialogueSOFirst;
    public DSDialogueSO dialogueSOLast;
    public ContainerParent parentGameObject;

    //public DSDialogueSO dialogueSO;
    
    //Old dialogue system parameters
    [SerializeField]
    public ContainerType containerType;
    
    public DictionaryDisplay[] branches;
    public DialogueContainer dialogueStateSwapper;
    public Dialogue dialogue;
    public bool useBranches;


    public QuestTrigger questTrigger;
    public List<GameObject> objectsToActivate;
    public List<GameObject> objectsToDeactivate;
    public SerializableDictionary<string, DialogueContainer> branchingOptions = new SerializableDictionary<string, DialogueContainer>();

    public int stateToSwapTo;
    public bool disableSelfDialogue;

    public bool useCustomFontSize;
    public float fontScale;
    public void Awake()
    {
       
        if (containerType == ContainerType.BranchingDialogue)
        {
            for (int i = 0; i < branches.Length; i++)
            {
                if (branches[i].name != null && branches[i].container != null)
                {
                    branchingOptions.Add(branches[i].name, branches[i].container);
                }
                else
                {
                    throw new NullReferenceException
                        ("Branches or name tags not equal, validate that branch names match the number of branches and that branches are assigned");
                }
           
            }
        }

        if (containerType == ContainerType.StateSwapper)
        {
            
        }

    }

    public SerializableDictionary<string, DialogueContainer> GetBranches()
    {
        return branchingOptions;
    }

    public void UpdateBranches()
    {
        branches = new DictionaryDisplay[branchingOptions.Count];
        int i = 0;
        foreach (KeyValuePair<string, DialogueContainer> entry in branchingOptions)
        {
            branches[i].container = entry.Value;
            branches[i].name = entry.Key;
            i++;
        }
    }
    
    


    public void SwapQuestState()
    {
        
        
        
        if (containerType == ContainerType.StateSwapper)
        {
            questTrigger.handler.SetCurrentQuestState(stateToSwapTo);
            questTrigger.RunQuestTriggersOnCurrentState();
        }
        else
        {
            Debug.LogError("Error: Dialogue State Swapper is not set to swapper but is attempting to swap quest states");
        }
        
    }
    
    

    /*public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (var branch in branchingOptions)
        {
            keys.Add(branch.Key);
            values.Add(branch.Value);
        }
    }*/

    /*public void OnAfterDeserialize()
    {
        branchingOptions = new Dictionary<string, DialogueContainer>();

        for (int i = 0; i != Math.Min(keys.Count, values.Count); i++)
        {
            branchingOptions.Add(keys[i], values[i]);
        }
    }

    private void OnGUI()
    {
        foreach (var branch in branchingOptions)
        {
            GUILayout.Label("Key: " + branch.Key + " Value: " + branch.Value);
        }
    }*/
}

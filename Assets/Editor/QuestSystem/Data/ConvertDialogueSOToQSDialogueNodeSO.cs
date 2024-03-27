using System.Collections;
using System.Collections.Generic;
using DS.Enumerations;
using DS.ScriptableObjects;
using DS.Utilities;
using UnityEditor;
using UnityEngine;

public class ConvertDialogueSOToQSDialogueNodeSO : ScriptableObject
{
    public string converterName;
    [NonReorderable]public List<DSDialogueSO> allDialogueNodes;
    [NonReorderable]public List<DSDialogueSO> exitDialogues;
    

    public void Initialize(string converterName)
    {
        this.converterName = converterName;
        allDialogueNodes = new List<DSDialogueSO>();
        exitDialogues = new List<DSDialogueSO>();
    }
    public void ConvertDSContainerToQuestNode(DSDialogueContainerSO dialogueContainerSo)
    {

        exitDialogues = new List<DSDialogueSO>();
        var listOfBranches = CreateListOfAllBranchListsInGraph(dialogueContainerSo);
        
        for (int i = 0; i < listOfBranches.Count; i++)
        {
            var currentList = listOfBranches[i];
            var last = currentList[^1];

            if (last.DialogueType != DSDialogueType.MultipleChoice)
            {
                //We're going to need a callback from the dialogue on closure that tells us which dialogue was the last one that played out and it needs to match the one that we got from this list.
                exitDialogues.Add(last);
                
            }
        }
       
        
    }

    private List<List<DSDialogueSO>> CreateListOfAllBranchListsInGraph(DSDialogueContainerSO dialogueContainerSo)
    {
        allDialogueNodes = new List<DSDialogueSO>();

        foreach (var node in dialogueContainerSo.ungroupedDialogues)
        {
            allDialogueNodes.Add(node);
        }

        foreach (KeyValuePair<DSDialogueGroupSO, List<DSDialogueSO>> groupedDialogues in dialogueContainerSo.dialogueGroups)
        {
            allDialogueNodes.AddRange(groupedDialogues.Value);
        }

        List<List<DSDialogueSO>> allLists = new List<List<DSDialogueSO>>();
        List<DSDialogueSO> currentList = new List<DSDialogueSO>();
        HashSet<DSDialogueSO> uniqueNodeSet = new HashSet<DSDialogueSO>();
        int iterationIndex = 0;

        var startingDialogue = allDialogueNodes.Find(e => e.isStartingDialogue);
        
        DSIOUtility.ConstructNodeListsRecursively(startingDialogue, allLists, currentList, uniqueNodeSet, iterationIndex);
        
        return allLists;
    }
}

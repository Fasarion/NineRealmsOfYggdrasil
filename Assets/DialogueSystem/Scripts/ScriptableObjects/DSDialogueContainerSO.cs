using System.Collections;
using System.Collections.Generic;
using DS.Enumerations;
using UnityEngine;
using DS.Utilities;

namespace DS.ScriptableObjects
{
    public class DSDialogueContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public SerializableDictionary<DSDialogueGroupSO, List<DSDialogueSO>> dialogueGroups { get; set; }
        [field: SerializeField] public List<DSDialogueSO> ungroupedDialogues { get; set; }

        public List<DSDialogueSO> allDialogueNodes;
        public List<DSDialogueSO> exitDialogues;
        
        
        public void Initialize(string fileName)
        {
            FileName = fileName;

            dialogueGroups = new SerializableDictionary<DSDialogueGroupSO, List<DSDialogueSO>>();
            ungroupedDialogues = new List<DSDialogueSO>();
            allDialogueNodes = new List<DSDialogueSO>();
            exitDialogues = new List<DSDialogueSO>();
            
            
        }
        
        public void ConvertDSContainerToQuestNode()
        {

            exitDialogues = new List<DSDialogueSO>();
            var listOfBranches = CreateListOfAllBranchListsInGraph(this);
        
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
        
        public static void ConstructNodeListsRecursively(DSDialogueSO dialogueSO, List<List<DSDialogueSO>> allLists, List<DSDialogueSO> currentList, HashSet<DSDialogueSO> uniqueNodeSet, int iterationIndex)
        {
            if (dialogueSO == null)
            {
                Debug.LogWarning("dialogueSO was empty, did you attempt to recurse through an empty graph?");
                return;
            }
            //Failsafe to keep me safe in my recursive moments.
            if (iterationIndex > 30)
            {
                Debug.LogWarning("Recursive function suspended prematurely to prevent infinite looping. Did you forget to remove the iteration limit?");
                return;
            }

           
            iterationIndex++;
            currentList.Add(dialogueSO);
            /*if (dialogueSo.isStartingDialogue)
            {
                branchKeys.Add(dialogueSo);
            }*/
            
            
            if (dialogueSO.Choices[0].NextDialogue == null)
            {
                allLists.Add(currentList);
                return;
            }
            if (dialogueSO.Choices.Count == 1)
            {
                var nextDialogue = dialogueSO.Choices[0].NextDialogue;

                if (uniqueNodeSet.Contains(nextDialogue))
                {
                    allLists.Add(currentList);
                    return;
                }

                uniqueNodeSet.Add(nextDialogue);
                ConstructNodeListsRecursively(nextDialogue, allLists, currentList,uniqueNodeSet,  iterationIndex);
                
            }
            else if(dialogueSO.Choices.Count > 1)
            {
                allLists.Add(currentList);
                foreach (var choice in dialogueSO.Choices)
                {
                    var nextDialogue = choice.NextDialogue;
                    if (uniqueNodeSet.Contains(nextDialogue))
                    {
                        continue;
                    }
                    uniqueNodeSet.Add(nextDialogue);
                    currentList = new List<DSDialogueSO>();
                    //branchKeys.Add(dialogueSo);
                    ConstructNodeListsRecursively(nextDialogue, allLists, currentList,uniqueNodeSet,iterationIndex);
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
        
            ConstructNodeListsRecursively(startingDialogue, allLists, currentList, uniqueNodeSet, iterationIndex);
            return allLists;
        }

        public List<string> GetDialogueGroupNames()
        {
            List<string> dialogueGroupNames = new List<string>();
            foreach (DSDialogueGroupSO dialogueGroup in dialogueGroups.Keys)
            {
                dialogueGroupNames.Add(dialogueGroup.GroupName);
                
            }

            return dialogueGroupNames;
        }

        public List<string> GetGroupedDialogueNames(DSDialogueGroupSO dialogueGroup, bool startingDialoguesOnly)
        {
            List<DSDialogueSO> groupedDialogues = dialogueGroups[dialogueGroup];
            List<string> groupedDialogueNames = new List<string>();

            foreach (DSDialogueSO groupedDialogue in groupedDialogues)
            {
                if (startingDialoguesOnly && !groupedDialogue.isStartingDialogue)
                {
                    continue;
                }
                groupedDialogueNames.Add(groupedDialogue.DialogueName);
            }

            return groupedDialogueNames;
        }

        public List<string> GetUngroupedDialogueNames(bool startingDialoguesOnly)
        {
            List<string> ungroupedDialogueNames = new List<string>();

            foreach (DSDialogueSO ungroupedDialogue in ungroupedDialogues)
            {
                if (startingDialoguesOnly && !ungroupedDialogue.isStartingDialogue)
                {
                    continue;
                }
                ungroupedDialogueNames.Add(ungroupedDialogue.DialogueName);
            }

            return ungroupedDialogueNames;
        }

    }
}


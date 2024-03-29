
using System.Collections.Generic;
using System.Linq;
using DS.Enumerations;
//using DS.Data.Save;
//using DS.Elements;
using DS.ScriptableObjects;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using DSDialogueGroupSO = DS.ScriptableObjects.DSDialogueGroupSO;

namespace DS
{

    public class Branch
    {
        public List<DSDialogueSO> dialogues;
        public DSDialogueSO origin;

        Branch()
        {
            dialogues = new List<DSDialogueSO>();
        }

        public void Add(DSDialogueSO dialogue)
        {
            dialogues.Add(dialogue);
        }
    }
    public class GraphToManagerConnector
    {
        private MonoBehaviour behaviour;

        private static DSDialogueContainerSO loadedDialogueContainer;
        private static List<DSDialogueSO> loadedUngroupedDialogues;
       
        private static List<DSDialogueSO> loadedGroupedDialogues;
        private static List<DSDialogueSO> allDialogueNodes;
        private static List<DSDialogueSO> firstNodes;
        private static List<DSDialogueSO> lastNodes;
        private DSDialogueSO startingDialogue;
        public string graphToLoad;

        private DialogueContainer containerPrefab;
        private ContainerParent containerParentPrefab;
        

        //private Dictionary<DSDialogueSO, List<DSDialogueSO>> allListsWithBranchingNodeAsKey;
        //private List<DSDialogueSO> branchKeys;

        public GraphToManagerConnector(DialogueContainer containerPrefab, ContainerParent containerParentPrefab)
        {
            loadedUngroupedDialogues = new List<DSDialogueSO>();
            loadedGroupedDialogues = new List<DSDialogueSO>();
            allDialogueNodes = new List<DSDialogueSO>();
            firstNodes = new List<DSDialogueSO>();
            lastNodes = new List<DSDialogueSO>();
            this.containerPrefab = containerPrefab;
            this.containerParentPrefab = containerParentPrefab;

            //allListsWithBranchingNodeAsKey = new Dictionary<DSDialogueSO, List<DSDialogueSO>>();
            //branchKeys = new List<DSDialogueSO>();

        }

        public Dictionary<string,ContainerParent> AddContainerGraphsToList(List<DSDialogueContainerSO> DSContainers, DialogueContainer dialogueContainer)
        {
            loadedUngroupedDialogues.Clear();
            loadedGroupedDialogues.Clear();
            allDialogueNodes.Clear();
            Dictionary<string, ContainerParent> graphContainerList = new Dictionary<string,ContainerParent>();
            //allListsWithBranchingNodeAsKey.Clear();
            //branchKeys.Clear();
            foreach (var dsDialogueContainerSO in DSContainers)
            {
                var containerParent = ConvertDSContainerToDialogueContainer(dsDialogueContainerSO, dialogueContainer);
                graphContainerList.Add(containerParent.name,containerParent);
            }

            return graphContainerList;



        }

        public ContainerParent ConvertDSContainerToDialogueContainer(DSDialogueContainerSO DSContainer, DialogueContainer dialogueContainer)
        {
            loadedDialogueContainer = DSContainer;
            loadedUngroupedDialogues.Clear();
            loadedGroupedDialogues.Clear();
            allDialogueNodes.Clear();
            foreach (var node in loadedDialogueContainer.ungroupedDialogues)
            {
                loadedUngroupedDialogues.Add(node);
            }
            
            
            foreach (KeyValuePair<DSDialogueGroupSO, List<DSDialogueSO>> groupedDialogues in loadedDialogueContainer.dialogueGroups)
            {
                loadedUngroupedDialogues.AddRange(groupedDialogues.Value);
            }
            allDialogueNodes.AddRange(loadedUngroupedDialogues);
            allDialogueNodes.AddRange(loadedGroupedDialogues);

            
            
            List<List<DSDialogueSO>> allLists = new List<List<DSDialogueSO>>();
            List<DSDialogueSO> currentList = new List<DSDialogueSO>();
            HashSet<DSDialogueSO> uniqueNodeSet = new HashSet<DSDialogueSO>();
            
            int iterationIndex = 0;

            foreach (DSDialogueSO node in allDialogueNodes)
            {
                if (node.isStartingDialogue)
                {
                    startingDialogue = node;
                }
            }
            
            ConstructNodeListsRecursively(startingDialogue, allLists, currentList, uniqueNodeSet, iterationIndex);

            /*if (branchKeys.Count == allLists.Count)
            {
                for(int i = 0; i < branchKeys.Count; i++)
                {
                    allListsWithBranchingNodeAsKey.Add(branchKeys[i], allLists[i]);
                }
            }
            else
            {
                Debug.LogError("Error! The number of keys in branchKeys was different from the number of lists in allLists");
            }*/


            List<DialogueContainer> branchContainerList = new List<DialogueContainer>();
            List<DialogueContainer> allContainers = new List<DialogueContainer>();
            var  parent = CreateRuntimeDialogueContainers(allLists, dialogueContainer, branchContainerList);
            parent.name = loadedDialogueContainer.name;
            return parent;
        }

        public ContainerParent CreateRuntimeDialogueContainers(List<List<DSDialogueSO>> allLists, DialogueContainer dialogueContainer, List<DialogueContainer> branchContainerList)
        {
            //Probably a little scuffed but it should run.

            List<DialogueContainer> allContainers = new List<DialogueContainer>();
            var parent = GameObject.Instantiate(containerParentPrefab);//GameObject.Instantiate(GameObject());
            
            for(int i = 0; i < allLists.Count; i++)
            {
                var currentList = allLists[i];
                var container = GameObject.Instantiate(dialogueContainer, parent.transform);
                
                List<Sentence> sentences = new List<Sentence>();
                foreach (DSDialogueSO node in  currentList)
                {
                    sentences.Add(new Sentence(node.Text));
                }

                var last = currentList[^1];
                var first = currentList[0];
                container.name = first.name;
                firstNodes.Add(first);
                lastNodes.Add(last);
                container.dialogueSOFirst = first;
                container.dialogueSOLast = last;
                if (last.DialogueType == DSDialogueType.MultipleChoice)
                {
                    container.containerType = ContainerType.BranchingDialogue;
                    
                    branchContainerList.Add(container);
                }
                else
                {
                    parent.exitContainers.Add(container);
                }
                container.dialogue = new Dialogue(sentences.ToArray());
                container.parentGameObject = parent;
                allContainers.Add(container);
            }
            //Forgive me for this deep level loop.
            //Go through all branches
            for (int i = 0; i < branchContainerList.Count; i++)
            {
                //Find the choices of the current branch
                var choices = branchContainerList[i].dialogueSOLast.Choices;
                for (int j = 0; j < choices.Count; j++)
                {
                    //For every choice found get the DialogueSO and go through all containers to find the first DialogueSO in that container and compare it to the choice.
                    //If they match, we add them to the container's branch list.
                    var next = choices[j].NextDialogue;
                    var found = allContainers.Find(e => e.dialogueSOFirst == next);
                    branchContainerList[i].branchingOptions.Add(next.name, found);

                }
                branchContainerList[i].UpdateBranches();
                
            }

            parent.containers = allContainers;
            

            return parent;
        }

        public void ConstructNodeListsRecursively(DSDialogueSO dialogueSo, List<List<DSDialogueSO>> allLists, List<DSDialogueSO> currentList, HashSet<DSDialogueSO> uniqueNodeSet, int iterationIndex)
        {
            
            //Failsafe to keep me safe in my recursive moments.
            if (iterationIndex > 30)
            {
                Debug.LogWarning("Recursive function suspended prematurely to prevent infinite looping. Did you forget to remove the iteration limit?");
                return;
            }

           
            iterationIndex++;
            currentList.Add(dialogueSo);
            /*if (dialogueSo.isStartingDialogue)
            {
                branchKeys.Add(dialogueSo);
            }*/
            
            
            if (dialogueSo.Choices[0].NextDialogue == null)
            {
                allLists.Add(currentList);
                return;
            }
            if (dialogueSo.Choices.Count == 1)
            {
                var nextDialogue = dialogueSo.Choices[0].NextDialogue;

                if (uniqueNodeSet.Contains(nextDialogue))
                {
                    allLists.Add(currentList);
                    return;
                }

                uniqueNodeSet.Add(nextDialogue);
                ConstructNodeListsRecursively(nextDialogue, allLists, currentList,uniqueNodeSet,  iterationIndex);
                
            }
            else if(dialogueSo.Choices.Count > 1)
            {
                allLists.Add(currentList);
                foreach (var choice in dialogueSo.Choices)
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
    }
}

        
       





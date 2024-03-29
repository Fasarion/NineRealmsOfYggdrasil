using System.Collections;
using System.Collections.Generic;
using DS.ScriptableObjects;
using KKD;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InternalDialogueGraphNode : InternalQuestNode
{
    private DSDialogueContainerSO dialogueContainerSO;


    private List<DialogueContainer> possibleContainers;
    private DialogueContainer chosenContainer;
    public InternalDialogueGraphNode(QuestHandler questHandler, InternalQuestNode nextQuestNode, DSDialogueContainerSO dialogueContainer) : base(questHandler, nextQuestNode)
    {
        dialogueContainerSO = dialogueContainer;
    }

    public void GetAllPossibleGraphChoices(ContainerParent containerParent)
    {
        possibleContainers = containerParent.exitContainers;
        foreach (var possibleContainer in possibleContainers)
        {
            //Create a set of node options that can be accessed.
            
        }
    }
    public void PickPlayerChosenQuestBranch(DialogueContainer container)
    {
        //We'll leave this for now and focus on the graph representation and we can build the solution for getting the graph working at runtime later.
        foreach (var possibleContainer in possibleContainers)
        {
            if (container == possibleContainer)
            {
                //What am I on about here eeh...
            }
        }
        //Go back to where the player inputs what option it's going for.
        
    }

}

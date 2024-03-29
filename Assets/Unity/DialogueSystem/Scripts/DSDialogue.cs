using System.Collections;
using System.Collections.Generic;
using DS.ScriptableObjects;
using UnityEngine;

namespace DS
{
    public class DSDialogue : MonoBehaviour
    {
        /* Dialogue Scriptable Objects */
        [SerializeField]private DSDialogueContainerSO dialogueContainer;
        [SerializeField]private DSDialogueGroupSO dialogueGroup;
        [SerializeField] private DSDialogueSO dialogue;
        
        /*Filters*/
        [SerializeField]private bool groupedDialogues;
        [SerializeField]private bool startingDialoguesOnly;

        /*Indexes*/
        [SerializeField]private int selectedDialogueGroupIndex;
        [SerializeField]private int selectedDialogueIndex;

    }
}

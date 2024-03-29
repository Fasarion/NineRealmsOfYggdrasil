using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCommand : Command
{
    private readonly DialogueManager dialogueManager;
    public DialogueCommand(GameObject receiver): base(receiver)
    {
        dialogueManager = this.receiver.GetComponent<DialogueManager>();
    }
    
    public override void Execute()
    {
        if (dialogueManager != null)
        {
            if (dialogueManager.branchChoiceDialogueBoxOpen)
            {
            
            }
            else
            {
                
                if (dialogueManager.dialogueOpen)
                {
                    dialogueManager.DisplayNextSentence();
                }
            }
        }
        
       
    }
}

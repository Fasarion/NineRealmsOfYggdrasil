using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public delegate void DialogueInteract();
public class TriggerDialogueCommand : Command
{
    //public static event DialogueInteract dialogueInteractedWith;
    private DialogueTriggerReceiver triggerReceiver;
    public TriggerDialogueCommand(GameObject receiver) : base(receiver)
    {
        triggerReceiver = receiver.GetComponent<DialogueTriggerReceiver>();
    }

    public override void Execute()
    {
        if (triggerReceiver.dialogueTrigger != null)
        {
            triggerReceiver.dialogueTrigger.OnInteract();
        }
       
        //dialogueInteractedWith?.Invoke();
    }

}

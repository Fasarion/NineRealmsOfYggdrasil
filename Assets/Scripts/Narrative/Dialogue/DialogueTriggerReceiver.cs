using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggerReceiver : MonoBehaviour
{
    [NonSerialized]
    public DialogueTrigger dialogueTrigger;

    public void ReceiveCurrentDialogueTrigger(DialogueTrigger trigger)
    {
        dialogueTrigger = trigger;
    }
}

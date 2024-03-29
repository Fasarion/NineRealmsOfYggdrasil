using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptDialogueTriggerTest : MonoBehaviour
{
    public DialogueTrigger trigger;
    public bool thingHasHappened;
    private bool hasTriggered;

    public void Update()
    {
        MakeStuffHappen();
    }

    private void MakeStuffHappen()
    {
        if (hasTriggered) return;
        if (!thingHasHappened) return;
        trigger.TriggerDialogue();
        hasTriggered = true;

    }
}

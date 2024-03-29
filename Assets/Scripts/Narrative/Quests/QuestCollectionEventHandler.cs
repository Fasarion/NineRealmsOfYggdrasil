using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestCollectionEventHandler", menuName = "EventHandlers/QuestCollectionEventHandler" + "")]
public class QuestCollectionEventHandler : ScriptableObject
{
    
    public void OnCollect(Component component)
    {
        //onCollect?.Invoke(component);
    }
}

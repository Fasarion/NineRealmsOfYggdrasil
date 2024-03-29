using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestKillable : MonoBehaviour
{
    [NonSerialized]public MonoBehaviour identityBehaviour;

    public QuestManager questManager;
    //public QuestCollectionEventHandler collectionEventHandler;
    public void CheckQuestKillableOnAttack()
    {
        questManager.CheckKillable(identityBehaviour);
        
    }
}

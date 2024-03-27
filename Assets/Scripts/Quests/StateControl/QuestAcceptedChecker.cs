using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestAcceptedChecker : MonoBehaviour
{
    [SerializeField]private QuestTrigger questTrigger;
    
    public void OnEnable()
    {
        questTrigger.QuestAccepted();
    }
}

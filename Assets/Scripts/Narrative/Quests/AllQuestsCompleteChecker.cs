using System.Collections;
using System.Collections.Generic;
using KKD;
using UnityEngine;

public class AllQuestsCompleteChecker : MonoBehaviour
{
    public List<GameObject> portalsToActivate;
    public QuestManager questManager;
    public void OnEnable()
    {
        questManager.onAllQuestTasksComplete += OnAllQuestTasksComplete;
        QuestHandler.onQuestTasksCompletedGlobal += OnQuestTasksCompletedGlobal;
    }

    private void OnAllQuestTasksComplete()
    {
        for (int i = 0; i < portalsToActivate.Count; i++)
        {
            portalsToActivate[i].SetActive(true);
            portalsToActivate[i].transform.parent = null;
        }
      
    }

    public void OnQuestTasksCompletedGlobal()
    {
        questManager.CheckAllNecessaryQuestsCompleted();
    }
}

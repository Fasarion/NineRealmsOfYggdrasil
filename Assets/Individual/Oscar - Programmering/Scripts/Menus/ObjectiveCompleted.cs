using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCompleted : MonoBehaviour
{
    public VictoryUI victoryUI;
    public float timeToWaitBeforeVictory;
    public void Start()
    {
        victoryUI = FindObjectOfType<VictoryUI>(true);
       
    }
    public void OnEnable()
    {
        EventManager.OnObjectiveReached += OnObjectiveReached;
    }
    
     public void OnDisable()
     {
         EventManager.OnObjectiveReached -= OnObjectiveReached;
     }
    

    private void OnObjectiveReached()
    {
        StartCoroutine(WaitForVictory());
       
    }

    private IEnumerator WaitForVictory()
    {
        bool needToWait = true;
        while (needToWait)
        {
            yield return new WaitForSeconds(timeToWaitBeforeVictory);

            needToWait = false;
        }
        victoryUI.gameObject.SetActive(true);
        Time.timeScale = 0;
        
       
        
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCompleted : MonoBehaviour
{
    public VictoryUI victoryUI;
    
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
        victoryUI.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
}

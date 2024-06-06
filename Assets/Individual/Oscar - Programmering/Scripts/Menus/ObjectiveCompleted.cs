using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCompleted : MonoBehaviour
{
    public GameObject victoryUI;
    public void OnEnable()
    {
        EventManager.OnObjectiveReached += OnObjectiveReached;
    }

    private void OnObjectiveReached()
    {
        victoryUI.SetActive(true);
        Time.timeScale = 0;
    }
}

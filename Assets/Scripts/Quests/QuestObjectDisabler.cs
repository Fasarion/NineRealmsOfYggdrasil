using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObjectDisabler : MonoBehaviour
{
    public GameObject objectToDisable;

    public void OnEnable()
    {
        objectToDisable.SetActive(false);
    }
}

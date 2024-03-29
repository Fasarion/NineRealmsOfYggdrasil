using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KKD;
using Unity.VisualScripting;
using UnityEngine;

public class QuestActiveChecker : MonoBehaviour
{
    public QuestHandler handler;
    public QuestTrigger trigger;
    private List<Transform> childTransforms;
    private List<GameObject> childObjects;

    public void Awake()
    {

        
        if (handler.questActive)
        {
            childTransforms = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                childTransforms.Add(transform.GetChild(i));
            }
            //childTransforms = transform.GetComponentsInChildren<Transform>(true).ToList();
            childObjects = new List<GameObject>();
            for (int i = 0; i < childTransforms.Count; i++)
            {
                childObjects.Add(childTransforms[i].gameObject);
            }
            trigger.gameObject.SetActive(true);
            trigger.questObjects = childObjects;
        }

        if (handler.questComplete)
        {
            trigger.gameObject.SetActive(false);
        }
    }

    public void OnEnable()
    {
        handler.onQuestActivated += OnQuestActivated;
    }

    public void OnDestroy()
    {
        handler.onQuestActivated -= OnQuestActivated;
    }

    public void OnQuestActivated(QuestHandler questHandler)
    {
        childTransforms = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            childTransforms.Add(transform.GetChild(i));
        }
        //childTransforms = transform.GetComponentsInChildren<Transform>(true).ToList();
        childObjects = new List<GameObject>();
        for (int i = 0; i < childTransforms.Count; i++)
        {
            childObjects.Add(childTransforms[i].gameObject);
        }
        trigger.gameObject.SetActive(true);
        trigger.questObjects = childObjects;
    }
}

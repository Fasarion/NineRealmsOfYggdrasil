using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DS.ScriptableObjects;
using KKD;
using UnityEditor;
using UnityEngine;

public class QuestRuntimeManager : MonoBehaviour
{
    private DialogueManager dialogueManager;
    public QSQuestDataContainerSO questData;

    public QSQuestSO currentNode;
    
    private QuestHandler questHandler;

    public Action<QuestHandler> sendQuestHandlerSetEvent;

    public bool conditionMet = false;


    private List<GameObject> gameObjectsInSceneAtStart; 
    // Start is called before the first frame update
    void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        
        if (questData != null)
        {
            if (questData.questHandlerSOs != null)
            {
                if (questData.questHandlerSOs.Count > 1)
                {
                    Debug.LogError("Error! Multiple quest handlers in the graph, quest will not function as expected.");
                }
                else
                {
                    currentNode = questData.startingNode;
                    var questNode = questData.questHandlerSOs[0];
                    questHandler = questNode.QuestHandler;
                }
            }
        }
    }

    private void OnEnable()
    {
        dialogueManager.reachedLastDialogueSO += OnReachedLastDialogueSO;
        questHandler.onQuestTasksCompleted += OnQuestConditionMet;
    }

    private void OnDisable()
    {
        dialogueManager.reachedLastDialogueSO -= OnReachedLastDialogueSO;
        questHandler.onQuestTasksCompleted -= OnQuestConditionMet;
    }

    private void Start()
    {
        //This is probably going to be VERY slow when we have 1000s of enemies in the scene. It would at least increase load times. Might have to resort to some kind of assignment logic for scene objects after all.
        //Could have a base class for all game objects that should be saved in the scene and add them into a list of objects to activate/deactivate.
        //Then we could leave spawned enemies created at runtime out of that list. Anyway, it is out of scope for the school assignment.
        gameObjectsInSceneAtStart = GetAllObjectsOnlyInScene();
        if (questHandler.questActive)
        {
            CheckNodeTransitionCondition();
        }
       
        
        //currentNode = questData.
        
    }

    public void OnQuestActivated(QuestHandler handler)
    {
        if (handler == questHandler)
        {
            CheckNodeTransitionCondition();
        }
    }

    public void AcceptQuest()
    {
        if (questHandler.questAccepted == false)
        {
            questHandler.questAccepted = true;
        }
    }

    List<GameObject> GetAllObjectsOnlyInScene()
    {
        List<GameObject> objectsInScene = new List<GameObject>();

        foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (!EditorUtility.IsPersistent(gameObject.transform.root.gameObject) && !(gameObject.hideFlags == HideFlags.NotEditable || gameObject.hideFlags == HideFlags.HideAndDontSave))
                objectsInScene.Add(gameObject);
        }

        return objectsInScene;
    }
    //Maybe I should make runtime classes for each node since they have logic attached to them?
    public void CheckNodeTransitionCondition()
    {
        if(currentNode.GetType() == typeof(QSActivatorSO))
        {
            var activatorNode = (QSActivatorSO)currentNode;
            
            ActivateAndDeactivateGameObjects(activatorNode);
            //activatorNode.GameObjectNames
        }
        else if (currentNode.GetType() == typeof(QSQuestAcceptedSO))
        {
            questHandler.questAccepted = true;
            GoToNextNode(currentNode.Branches[0]);
        }
        else if (currentNode.GetType() == typeof(QSQuestActivatorSO))
        {
            var questActivatorNode = (QSQuestActivatorSO)currentNode;
            questActivatorNode.QuestHandler.QuestActivated();
            
            GoToNextNode(currentNode.Branches[0]);
        }
        else if (currentNode.GetType() == typeof(QSQuestHandlerSO))
        {
            //Should not be possible.
        }
        else if (currentNode.GetType() == typeof(QSConditionSetterSO))
        {
            GoToNextNode(currentNode.Branches[0]);
            if (questHandler.questType == QuestType.TalkToQuest)
            {
                questHandler.QuestTasksCompleted();
            }
            
            
        }
        else if (currentNode.GetType() == typeof(QSConditionSO))
        {
            if (conditionMet)
            {
                GoToNextNode(currentNode.Branches[0]);
            }
            
        }

    }

    public void ActivateAndDeactivateGameObjects(QSActivatorSO activatorNode)
    {

        //gameObjectsInSceneAtStart.RemoveAll(e => e == null);
        for (int i = 0; i < gameObjectsInSceneAtStart.Count; i++)
        {
            var currentObject = gameObjectsInSceneAtStart[i].gameObject;
            foreach (var objectName in activatorNode.GameObjectsToActivateNames)
            {
                if (currentObject != null)
                {
                    if (objectName == currentObject.name)
                    {
                        currentObject.SetActive(true);
                    }
                }
                
            }
            foreach (var objectName in activatorNode.GameObjectsToDeactivateNames)
            {
                if (currentObject != null)
                {
                    if (objectName == currentObject.name)
                    {
                        currentObject.SetActive(false);
                    }
                }
               
            }
        };
        
        GoToNextNode(currentNode.Branches[0]);
    }
    

    public void OnReachedLastDialogueSO(DSDialogueSO dialogueSo)
    {
        SelectDialogueGraphBranch(dialogueSo);
    }

    public void OnQuestConditionMet()
    {
        //Debug.Log("Condition fulfilled!");
        conditionMet = true;
        if (currentNode.GetType() == typeof(QSConditionSO))
        {
            GoToNextNode(currentNode.Branches[0]);
        }
    }
    //We have to set the chosen path based on what dialogue is played out.
    public void SelectDialogueGraphBranch(DSDialogueSO dialogueSo)
    {
        if (currentNode is QSDialogueGraphSO dialogueGraphSo)
        {
            var found = dialogueGraphSo.DialogueContainerSO.exitDialogues.Find(e => e == dialogueSo);
            QSQuestBranchData chosenBranch = null;
            if (found != null)
            {
                foreach (QSQuestBranchData qsQuestBranchData in dialogueGraphSo.Branches)
                {
                    if (qsQuestBranchData.Text == found.name)
                    {
                        chosenBranch = qsQuestBranchData;
                    }
                }

                if (chosenBranch != null)
                {
                   
                    GoToNextNode(chosenBranch);
                    
                }
                else
                {
                    Debug.LogWarning("No branch was found to transition to, is this intended?");
                }
            }
        }
    }
    
    public void GoToNextNode(QSQuestBranchData questBranch)
    { 
        if (questBranch != null)
        {
            currentNode = questBranch.NextQuestNode;
            if (currentNode != null)
            {
                CheckNodeTransitionCondition();
            }
            else
            {
                if (questHandler.questAccepted)
                {
                    questHandler.QuestCompleted(questHandler);
                }
                else
                {
                    currentNode = questData.startingNode;
                    CheckNodeTransitionCondition();
                }
                
                
            }
            
        }
        else
        {
            if (questHandler.questAccepted)
            {
                questHandler.QuestCompleted(questHandler);
            }
            else
            {
                currentNode = questData.startingNode;
                CheckNodeTransitionCondition();
            }
            
        }
    }
}

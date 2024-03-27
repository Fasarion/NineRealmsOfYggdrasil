using System.Collections;
using System.Collections.Generic;
using KKD;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestHandler))]
public class QuestHandlerEditor : Editor
{

    private SerializedProperty selectQuestType;
    private SerializedProperty currentQuestState;
    private SerializedProperty questActive;
    private SerializedProperty firstTimeAccepting;
    private SerializedProperty questAccepted;
    private SerializedProperty questTaskComplete;
    private SerializedProperty questComplete;
    private SerializedProperty currentCollectedItems;
    private SerializedProperty currentKills;
    private SerializedProperty fetchItemRetrieved;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Cached properties that are common to all quests.
        selectQuestType = serializedObject.FindProperty("questType");
        currentQuestState = serializedObject.FindProperty("currentQuestState");
        questActive = serializedObject.FindProperty("questActive");
        firstTimeAccepting = serializedObject.FindProperty("firstTimeAccepting");
        questAccepted = serializedObject.FindProperty("questAccepted");
        questTaskComplete = serializedObject.FindProperty("questTasksComplete");
        questComplete = serializedObject.FindProperty("questComplete");
        
        //Cached properties that relate to a particular quest type.
        currentCollectedItems = serializedObject.FindProperty("currentCollectedItems");
        currentKills = serializedObject.FindProperty("currentKills");
        fetchItemRetrieved = serializedObject.FindProperty("fetchItemRetrieved");
        
        //Fields for cached properties used by all quest types that are visible in the inspector
        EditorGUILayout.PropertyField(selectQuestType);
        EditorGUILayout.PropertyField(currentQuestState);
        EditorGUILayout.PropertyField(questActive);
        EditorGUILayout.PropertyField(firstTimeAccepting);
        EditorGUILayout.PropertyField(questAccepted);
        EditorGUILayout.PropertyField(questTaskComplete);
        EditorGUILayout.PropertyField(questComplete);
        
        //Fields for cached properties relating to a particular quest type that are visible in the inspector
        switch (selectQuestType.enumValueFlag)
        {
            case (int)QuestType.CollectQuest:
              
                EditorGUILayout.PropertyField(serializedObject.FindProperty("collectableItemName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("itemTypesToCollect"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("startingCollectedItems"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("numberOfItemsToCollect"));
                EditorGUILayout.PropertyField(currentCollectedItems);
                
                break;
            case (int)QuestType.KillQuest:
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("enemyTypeName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("enemyTypeToKill"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("startingKills"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("killGoal"));
                EditorGUILayout.PropertyField(currentKills);
                break;
            case (int)QuestType.FetchQuest:
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("itemToFetchName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("characterToFetchFrom"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("characterToFetchFor"));
                EditorGUILayout.PropertyField(fetchItemRetrieved);
                break;
            case (int)QuestType.TalkToQuest:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("characterToTalkTo"));
                break;
        }

        //Button layout
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset"))
        {
            ResetQuest(true);
        }
        if (GUILayout.Button("Reset To Inactive"))
        {
            ResetQuest(false);
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    public void ResetQuest(bool questActiveState)
    {
        currentQuestState.intValue = 0;
        questActive.boolValue = questActiveState;
        firstTimeAccepting.boolValue = true;
        questAccepted.boolValue = false;
        questTaskComplete.boolValue = false;
        questComplete.boolValue = false;
        currentCollectedItems.intValue = 0;
        currentKills.intValue = 0;
        fetchItemRetrieved.boolValue = false;
    }

}

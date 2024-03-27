using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;


[CustomEditor(typeof(DialogueTrigger))]
public class DialogueTriggerEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        //FMODUnity.RuntimeManager.CreateInstance()
        serializedObject.Update();
        var selectTriggerType = serializedObject.FindProperty("triggerType");
        var selectCounterType = serializedObject.FindProperty("counterType");
        var selectDialogueSystemType = serializedObject.FindProperty("dialogueSystemType");
        var dialogueContainerScriptableObject = serializedObject.FindProperty("dialogueContainerScriptableObject");
        EditorGUILayout.PropertyField(selectDialogueSystemType);

        switch (selectDialogueSystemType.enumValueFlag)
        {
            case (int)DialogueSystemType.DialogueGraph:
            {
                EditorGUILayout.PropertyField(selectTriggerType);
                EditorGUILayout.PropertyField(dialogueContainerScriptableObject);
                DrawCommonUI(selectTriggerType, selectCounterType);
                break;
            }
            
            case (int)DialogueSystemType.Legacy:
            {
                EditorGUILayout.PropertyField(selectTriggerType);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("startingDialogueBranch"));
                DrawCommonUI(selectTriggerType, selectCounterType);
                break;
            }
        }
        

        serializedObject.ApplyModifiedProperties();

    }

    private void DrawCommonUI(SerializedProperty selectTriggerType, SerializedProperty selectCounterType)
    {
        
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("destroySelfOnTrigger"));
        
        
        
        switch (selectTriggerType.enumValueFlag)
        {
            case (int)TriggerType.Interaction:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("interactUIText"));
                break;
            case (int)TriggerType.Collider:
                //EditorGUILayout.PropertyField(serializedObject.FindProperty("destroySelfOnTrigger"));
                break;
            case (int)TriggerType.Counter:
                
                
                EditorGUILayout.PropertyField(selectCounterType);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("countingDown"));
                switch (selectCounterType.enumValueFlag)
                {
                    case (int)CounterType.Gold:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("goldHandler"));
                        break;
                    case (int)CounterType.Health:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("healthHandler"));
                        break;
                    case (int)CounterType.XP:
                    case (int)CounterType.Level:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("xpHandler"));
                        break;
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("counter"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("countGoal"));
                break;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*[CustomEditor(typeof(DialogueContainer))]
public class DialogueContainerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        //var myScript = target as DialogueContainer;
        //myScript.useBranches = EditorGUILayout.Toggle("Use Branches",myScript.useBranches);
        //var branchesToggle = serializedObject.FindProperty("useBranches");
        serializedObject.Update();
        var selectContainerType = serializedObject.FindProperty("containerType");
        var useCustomFontSize = serializedObject.FindProperty("useCustomFontSize");
        //EditorGUILayout.tog
        
        //EditorGUILayout.PropertyField(branchesToggle);
        EditorGUILayout.PropertyField(selectContainerType);//EditorList.Show(serializedObject.FindProperty("useBranches"));
       
        
        
        
        if(selectContainerType.enumValueFlag == (int)ContainerType.StateSwapper)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("questTrigger"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stateToSwapTo"));
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogue"));
            
            if (selectContainerType.enumValueFlag == (int)ContainerType.DialogueWithStateSwapper)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogueStateSwapper"));
            }
            
            if (selectContainerType.enumValueFlag == (int)ContainerType.BranchingDialogue/*myScript.useBranches)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("branches"));
            }
        }

        EditorGUILayout.PropertyField(useCustomFontSize);
        if (useCustomFontSize.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fontScale"));
        }
        
        //EditorList.Show(serializedObject.FindProperty("keys"));
        //EditorList.Show(serializedObject.FindProperty("values"));
        serializedObject.ApplyModifiedProperties();
        
        
    }
}*/

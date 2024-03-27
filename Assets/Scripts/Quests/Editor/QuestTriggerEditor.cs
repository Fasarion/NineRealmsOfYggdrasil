using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestTrigger))]
public class QuestTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("handler"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("questUIUpdate"));
        EditorList.Show(serializedObject.FindProperty("questStatesList"));
        serializedObject.ApplyModifiedProperties();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorList
{
    public static void Show(SerializedProperty list)
    {
        EditorGUILayout.PropertyField(list);
        //EditorGUILayout.PrefixLabel(list.displayName);
        //EditorGUILayout.PropertyField(list)
        //EditorGUILayout. 
        //EditorGUI.indentLevel += 10;
        EditorGUI.indentLevel += 1;
        for (int i = 0; i < list.arraySize; i++)
        {
            EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
        }
        EditorGUI.indentLevel -= 1;

        //EditorGUI.indentLevel -= 10;

    }
}

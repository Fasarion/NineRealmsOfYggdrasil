using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(QuestStates))]
public class QuestStatesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);
        EditorGUI.indentLevel = 0;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("objectsToActivate"),new GUIContent("ObjectToActivate"));
        
        contentPosition.y += contentPosition.height;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("objectsToDeactivate"),new GUIContent("ObjectToDeactivate"));
        EditorGUI.EndProperty();
        
        
        //EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("objectsToActivate"),GUIContent.none);
        
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return Screen.width < 333 ? (16f + 18f) : 16f;
    }
}

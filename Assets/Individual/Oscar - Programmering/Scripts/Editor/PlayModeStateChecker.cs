using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine;
using UnityEditor;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoadAttribute]
public static class PlayModeStateChecker
{
    /*public static ChoiceDataScriptableObject choiceData;
    static PlayModeStateChecker()
    {
        if (choiceData == null)
        {
            choiceData = AssetDatabase.LoadAssetAtPath<ChoiceDataScriptableObject>("Assets/Individual/Oscar - Programmering/ScriptableObjects/ChoiceData/ChoiceData");

        }
        
        
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        Debug.Log("Loaded Choice Data");
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Debug.Log("PlayModeExited");
            choiceData.currentRoomNode = null;
        }
    }*/
}

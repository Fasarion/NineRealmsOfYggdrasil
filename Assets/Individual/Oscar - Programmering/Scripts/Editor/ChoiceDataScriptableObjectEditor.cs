using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChoiceDataScriptableObject))]
public class ChoiceDataScriptableObjectEditor : Editor
{
    private ChoiceDataScriptableObject targetObject;

    private void OnEnable()
    {
        targetObject = (ChoiceDataScriptableObject)target;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode && targetObject != null)
        {
            // Clear the reference to your non-MonoBehaviour class here
            if (targetObject.resetNodeProgression)
            {
                targetObject.currentRoomNode = null;
            }
            
            
            // Mark the object as dirty so changes are saved
            EditorUtility.SetDirty(targetObject);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Optionally, you can add custom GUI elements here if needed
    }
}

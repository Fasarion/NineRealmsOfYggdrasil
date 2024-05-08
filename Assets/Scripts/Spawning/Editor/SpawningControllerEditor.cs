using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawningController))]
public class SpawningControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SpawningController script = (SpawningController)target;

        if (GUILayout.Button("Update Spawner Checkpoints"))
        {
            // Call the method or perform the action when the button is pressed
            script.UpdateSpawningCheckpoints();
        }
    }
}

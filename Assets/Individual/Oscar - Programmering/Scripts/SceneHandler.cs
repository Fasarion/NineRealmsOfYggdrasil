using System.Collections;
using System.Collections.Generic;
using DevLocker.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public SceneReference sceneToLoad;
    public void SwapScene()
    {
        SceneManager.LoadScene(sceneToLoad.ScenePath);
    }

    public void ExitGame()
    {
        if (Application.isEditor)
        {
            EditorApplication.isPlaying = false;
        }
        Application.Quit();
    }
}

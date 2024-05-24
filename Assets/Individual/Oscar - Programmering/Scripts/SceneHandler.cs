using System.Collections;
using System.Collections.Generic;
using DevLocker.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public SceneReference _sceneReference;
    public void SwapScene()
    {
        SceneManager.LoadScene(_sceneReference.ScenePath);
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

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void SwapScene(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
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

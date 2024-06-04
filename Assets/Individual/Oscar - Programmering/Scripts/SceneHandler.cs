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
    
        #if UNITY_EDITOR
            EditorUtility.DisplayDialog("Great!", "You got the pattern right!", "Next Level!");
            if (Application.isEditor)
            {
                EditorApplication.isPlaying = false;
            }
      
           
        #endif
        #if !UNITY_EDITOR
        Application.Quit();
        #endif
       
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatSceneSwapper : MonoBehaviour
{
    public Scene sceneToSwapTo;
    public void SwapScene()
    {
        SceneManager.LoadScene("OscarSceneStart");
    }
}

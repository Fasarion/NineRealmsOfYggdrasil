using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMenuSceneManager : MonoBehaviour
{ 
    public SceneHandler continueSceneHandler;
    public SceneHandler restartSceneHandler;
    public SceneHandler exitToMenuSceneHandler;

    public FadeHandler fadeHandler;
   

    private MenuButtonSelection menuButtonSelection;

    public void Start()
    {
        menuButtonSelection = MenuButtonSelection.Start;
        fadeHandler.FadeIn();
       
    }

    public void OnEnable()
    {
        EventManager.OnScreenFadeComplete += OnFadeComplete;
    }
    
    public void OnDisable()
    {
        EventManager.OnScreenFadeComplete -= OnFadeComplete;
    }

    public void Continue()
    {
        menuButtonSelection = MenuButtonSelection.Continue;
        fadeHandler.FadeOut();
    }
    
    public void Restart()
    {
        menuButtonSelection = MenuButtonSelection.Restart;
        fadeHandler.FadeOut();
    }
    public void ExitToMenu()
    {
        menuButtonSelection = MenuButtonSelection.ExitToMenu;
        fadeHandler.FadeOut();
    }
   
    public void ExitGame()
    {
        menuButtonSelection = MenuButtonSelection.ExitGame;
        fadeHandler.FadeOut();
    }

   

   private void OnFadeComplete()
   {
       switch (menuButtonSelection)
       {
           case MenuButtonSelection.Continue:
           {
               continueSceneHandler.SwapScene();
               break;
           }
           case MenuButtonSelection.Restart:
           {
               restartSceneHandler.SwapScene();
               break;
           }
           case MenuButtonSelection.ExitToMenu:
           {
               exitToMenuSceneHandler.SwapScene();
               break;
           }
           case MenuButtonSelection.ExitGame:
           {
               exitToMenuSceneHandler.ExitGame();
               break;
           }
       }
       
   }
   // Start is called before the first frame update
   

   // Update is called once per frame
   void Update()
   {
       
   }
   
}

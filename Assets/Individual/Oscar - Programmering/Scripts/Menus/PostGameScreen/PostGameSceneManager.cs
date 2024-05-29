using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PostGameSceneManager : MonoBehaviour
{
    public SceneHandler continueSceneHandler;
    public SceneHandler exitToMenuSceneHandler;

    public FadeHandler fadeHandler;
    private enum ButtonSelection
    {
        
        Start,
        Continue,
        ExitToMenu,
        ExitGame
    }

    private ButtonSelection buttonSelection;

    public void Start()
    {
        buttonSelection = ButtonSelection.Start;
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
        buttonSelection = ButtonSelection.Continue;
        fadeHandler.FadeOut();
    }
    public void ExitToMenu()
    {
        buttonSelection = ButtonSelection.ExitToMenu;
        fadeHandler.FadeOut();
    }
    
    public void ExitGame()
    {
        buttonSelection = ButtonSelection.ExitGame;
        fadeHandler.FadeOut();
    }

    

    private void OnFadeComplete()
    {
        switch (buttonSelection)
        {
            case ButtonSelection.Continue:
            {
                continueSceneHandler.SwapScene();
                break;
            }
            case ButtonSelection.ExitToMenu:
            {
                exitToMenuSceneHandler.SwapScene();
                break;
            }
            case ButtonSelection.ExitGame:
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject audioSettingsMenu;
    
    public void Awake()
    {
        DeactivateAudioSettings();
    }
    
    public void ActivateAudioSettings()
    {
        audioSettingsMenu.SetActive(true);
    }
    
    public void DeactivateAudioSettings()
    {
        audioSettingsMenu.SetActive(false);
    }
}

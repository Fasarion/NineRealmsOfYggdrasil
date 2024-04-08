using System;
using System.Collections;
using System.Collections.Generic;
using DevLocker.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomChoiceUIManager : MonoBehaviour
{
    
    private static RoomChoiceUIManager _instance;
    
    [SerializeField] private List<RoomSelectionUIBehaviour> roomSelects;
    //This is probably very ineffective, fix.
    [SerializeField] private List<RoomChoiceObject> roomChoiceObjects;
    private bool _isUIDisplayed;
    
    public Action<SceneReference> OnRoomChosen;
    public static RoomChoiceUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RoomChoiceUIManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            //Destroy(gameObject);
        }
        
        //HideUI();
    }

    public void Start()
    {
        DisplayRoomChoiceTree(roomChoiceObjects);
    }
    
    private void DisplayRoomChoiceTree(List<RoomChoiceObject> roomChoiceObjects)
    {
        ShowUI(roomChoiceObjects.Count);
        
        for (int i = 0; i < roomChoiceObjects.Count; i++)
        {
            RoomChoiceObject roomChoice = roomChoiceObjects[i];
            roomSelects[i].UpdateRoomSelectionDisplay(roomChoice);
        }
    }
    
    
    private void OnEnable()
    {
        
        //var upgradeUISystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<UpgradeUISystem>();
        //upgradeUISystem.OnUpgradeUIDisplayCall += DisplayUpgradeCards;
    }

    private void OnDisable()
    {
        //if (World.DefaultGameObjectInjectionWorld == null) return;
        //var upgradeUISystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<UpgradeUISystem>();
        //upgradeUISystem.OnUpgradeUIDisplayCall -= DisplayUpgradeCards;
    }
    

    private void ShowUI(int roomCount)
    {
        for (int i = 0; i < roomCount; i++)
        {
            roomSelects[i].gameObject.SetActive(true);
        }

        _isUIDisplayed = true;
        Time.timeScale = 0f;
    }

    private void HideUI()
    {
        foreach (var room in roomSelects)
        {
            room.gameObject.SetActive(false);
        }

        _isUIDisplayed = false;
        Time.timeScale = 1f;
    }
    
    
    public void RegisterRoomSelectionClick(SceneReference roomSceneReference)
    {
        HideUI();
       
        OnRoomChosen?.Invoke(roomSceneReference);
        SceneManager.LoadScene(roomSceneReference.SceneName);
    }
}

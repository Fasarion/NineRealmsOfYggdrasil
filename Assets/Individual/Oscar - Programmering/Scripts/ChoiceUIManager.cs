using System;
using System.Collections;
using System.Collections.Generic;
using DevLocker.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChoiceUIManager : MonoBehaviour
{

    public enum SelectionType
    {
        roomChoice,
        weaponChoice,
        shopChoice
    }

    [SerializeField] private SelectionType selectionType;
    
    private static ChoiceUIManager _instance;
    
    [SerializeField] private List<RoomChoiceUIBehaviour> roomSelects;
    //This is probably very ineffective, fix.
    [SerializeField] private List<RoomChoiceObject> roomChoiceObjects;

    [SerializeField] private SelectionCardInstantiator roomSelectionCardsInstantiator;
    [SerializeField] private SelectionCardInstantiator weaponSelectionCardsInstantiator;
    [SerializeField] private SelectionCardInstantiator shopSelectionCardsInstantiator;
    

    private int currentSelectionIndex;
    private int maxSelectionIndex = 2;
    
    
    private bool _isUIDisplayed;
    
    
    
    public Action<SceneReference> OnRoomChosen;
    public static ChoiceUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ChoiceUIManager>();
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
        roomSelectionCardsInstantiator.InstantiateSelectionCards(3);
        shopSelectionCardsInstantiator.InstantiateSelectionCards(3);
        weaponSelectionCardsInstantiator.InstantiateSelectionCards(3);
        weaponSelectionCardsInstantiator.MoveSelectionCardsIntoView();
        //DisplayRoomChoiceTree(roomChoiceObjects);
    }
    
    private void DisplayRoomChoiceTree(List<RoomChoiceObject> roomChoiceObjects)
    {
        ShowUI(roomChoiceObjects.Count);
        
        for (int i = 0; i < roomChoiceObjects.Count; i++)
        {
            RoomChoiceObject roomChoice = roomChoiceObjects[i];
            roomSelects[i].UpdateSelectionDisplay(roomChoice);
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

    public void SwapScreenRight()
    {
        if (currentSelectionIndex < maxSelectionIndex)
        {
            currentSelectionIndex++;
        }
        UpdateSelectionType(currentSelectionIndex);
    }
    
    public void SwapScreenLeft()
    {
        if (currentSelectionIndex > 0)
        {
            currentSelectionIndex--;
        }
        UpdateSelectionType(currentSelectionIndex);
    }
    
    public void UpdateSelectionType(int index)
    {
        switch (index)
        {
            case 0:
            {
                selectionType = SelectionType.roomChoice;
                UpdateSelectionScreen();
                break;
            }
            case 1:
            {
                selectionType = SelectionType.weaponChoice;
                UpdateSelectionScreen();
                break;
            }
            case 2:
            {
                selectionType = SelectionType.shopChoice;
                UpdateSelectionScreen();
                break;
            }
            default:
            {
                Debug.LogError("Error! Index out of scope. Did you add an enum for that index?");
                break;
            }
            
          
        }
    }
    

    public void UpdateSelectionScreen()
    {
        switch (selectionType)
        {
            case SelectionType.roomChoice:
            {
                roomSelectionCardsInstantiator.gameObject.SetActive(true);
                weaponSelectionCardsInstantiator.gameObject.SetActive(false);
                shopSelectionCardsInstantiator.gameObject.SetActive(false);
                break;
            }
            case SelectionType.shopChoice:
            {
                roomSelectionCardsInstantiator.gameObject.SetActive(false);
                weaponSelectionCardsInstantiator.gameObject.SetActive(false);
                shopSelectionCardsInstantiator.gameObject.SetActive(true);
                break;
            }
            case SelectionType.weaponChoice:
            {
             
                roomSelectionCardsInstantiator.gameObject.SetActive(false);
                weaponSelectionCardsInstantiator.gameObject.SetActive(true);
                shopSelectionCardsInstantiator.gameObject.SetActive(false);
                break;
            }
        }
    }
    
    /*public void OnSelectionTypeClick()
    {
        switch (selectionType)
        {
            case SelectionType.roomChoice:
            {
                break;
            }
            case SelectionType.shopChoice:
            {
                break;
            }
            case SelectionType.weaponChoice:
            {
                break;
            }
        }
    }*/
    
    public void RegisterRoomSelectionClick(SceneReference roomSceneReference)
    {
        HideUI();
       
        OnRoomChosen?.Invoke(roomSceneReference);
        SceneManager.LoadScene(roomSceneReference.SceneName);
    }
}
